using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Point = System.Drawing.PointF;
using System.Linq;
using Utils;
using Utils.Attributes;
using Utils.Converters;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModelBase
{
    public class LineIntersectsFinder
    {
        public List<(Line, Line)> intersected;
    }

    public class StepBase
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Wait()
        {
            // Nothing
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Step()
        {
            // Nothing
        }
    }

    public class StepSync : StepBase
    {
        public static StepSync Instance { get; } = new StepSync();
        private System.Threading.AutoResetEvent ev = new System.Threading.AutoResetEvent(false);
        private bool enabled = true;
        private readonly object __lock = new object();

        public bool EnableDebugBreak { get; set; }

        public bool EnableWaiting
        {
            get => enabled;
            set
            {
                if (value)
                {
                    enabled = true;
                    ev.Reset();
                }
                else
                {
                    enabled = false;
                    ev.Set();
                }
            }
        }

        public override void Wait()
        {
            lock (__lock) // Чтобы избежать ситуации, когда enabled выставляется в false,
                          // а поток успевает уйти в ожидание
            {
                if (enabled)
                {
                    ev.Reset(); // Сброс
                    OnEntry(); // Вызов события Entry, для уведомления подписчикам
                    ev.WaitOne(); // Блокировка потока
                    if (EnableDebugBreak && enabled) Debugger.Break();
                }
            }
        }

        public override void Step()
        {
            ev.Set();
            OnSteped();
        }

        protected virtual void OnEntry()
        {
            if (Entry != null)
            {
                var st = new StackTrace(2, true);
                Entry(st);
            }
        }

        protected virtual void OnSteped()
        {
            Steped?.Invoke();
        }

        public event Action<StackTrace> Entry;

        public event Action Steped;
    }

    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Map : MarshalByRefObject
    {
        static Map instance;
        public static Map Instance 
        {
            get => instance ?? (instance = new Map());
            set => instance = value; 
        }

        #region PublicMembers

        #region Properties

        /// <summary>
        /// Список точек, размещенных на карте
        /// </summary>
        [EditableProperty(nameof(Dot.Point), typeof(Point), typeof(PointFConverter))]
        [TypeConverter(typeof(ReadOnlyDictionaryTypeConverter))]
        [DisplayName("Точки")]
        [Description("Список точек, размещенных на карте")]
        public IReadOnlyDictionary<int, Dot> Dots { get; }

        /// <summary>
        /// Список линий, которые соединяют точки
        /// </summary>
        //[EditableProperty(nameof(Line.), typeof(PointFConverter))]
        [Expandable]
        [TypeConverter(typeof(ReadOnlyDictionaryTypeConverter))]
        [DisplayName("Линии")]
        [Description("Список линий, которые соединяют точки")]
        public IReadOnlyDictionary<int, Line> LineContainer { get; }

        /// <summary>
        /// Список объектов, которые получились здесь
        /// </summary>
        [Expandable]
        [TypeConverter(typeof(ReadOnlyDictionaryTypeConverter))]
        [DisplayName("Объекты")]
        [Description("Список объектов, которые получились здесь")]
        public IReadOnlyDictionary<int, LineSet> LineSets { get; }

        /// <summary>
        /// Список пересекаемых линий на карте
        /// </summary>
        [DisplayName("Пересечения линий")]
        [Description("Список пересекаемых линий на карте")]
        [ReadOnly(true)]
        [TypeConverter(typeof(ReadOnlyDictionaryTypeConverter))]
        [EditableProperty("", typeof(Line[]), typeof(ArrayConverter))]
        [Expandable]
        public Dictionary<Line, Line[]> LineIntersects { get; set; }

        /// <summary>
        /// Автоматически находить объекты на карте при перемещении точек
        /// </summary>
        [DisplayName("Перестраивать линии")]
        [Description("Автоматически находить объекты на карте при перемещении точек")]
        public bool AutoRebuildLineSets { get; set; } = true;

        /// <summary>
        /// Автоматически находить отношения между фигурами при перемещении точек
        /// </summary>
        [DisplayName("Перестраивать отношения")]
        [Description("Автоматически находить отношения между фигурами при перемещении точек")]
        public bool AutoRefindRelations { get; set; } = true;

        /// <summary>
        /// Визуализация отношений вложености на карте
        /// </summary>
        [DisplayName("Отображать вложеность")]
        [Description("Визуализация отношений вложености на карте")]
        public bool FillRelations { get; set; } = false;

        public (Line, Line)[] Intersects = new (Line, Line)[0];

        [Browsable(false)]
        public bool ConnectLong { get; set; }

        [Browsable(false)]
        public bool CasheDotDistances { get; set; } = true;

        [Browsable(false)]
        public Func<Dot[], (float dist, Dot d1, Dot d2)[], IEnumerable<LineSet>> CustomBuildLines { get; set; }

        [Browsable(false)]
        public Func<(float dist, Dot d1, Dot d2), CancellationToken, StepBase, bool> ConnectCustom { get; set; }

        [Browsable(false)]
        public Action<(float dist, Dot d1, Dot d2)[], CancellationToken, StepBase> ConnectInArrayCustom { get; set; }

        #endregion Properties

        #region Methods

        public void AutoFind(CancellationToken cancellationToken = default)
        {
            if (AutoRebuildLineSets)
            {
                BuildLineSets(cancellationToken);
            }
            if (AutoRefindRelations)
            {
                FindRelations(cancellationToken);
            }
        }

        public Func<(float dist, Dot d1, Dot d2), CancellationToken, StepBase, bool> GetConnect()
        {
            return ConnectCustom ?? ((item, ct, step) =>
               {
                   if (item.d1.secondConnect != null && item.d2.secondConnect != null) return false;
                   step.Wait();
                   if (item.d1.firstConnect == null) item.d1.firstConnect = item.d2;
                   else item.d1.secondConnect = item.d2;
                   if (item.d2.firstConnect == null) item.d2.firstConnect = item.d1;
                   else item.d2.secondConnect = item.d1;
                   return true;
               });
        }

        public Action<(float dist, Dot d1, Dot d2)[], CancellationToken, StepBase> GetConnectInArray()
        {
            return ConnectInArrayCustom ?? ((sortedDist, ct, step) =>
                {
                    Func<(float dist, Dot d1, Dot d2), CancellationToken, StepBase, bool> connect = GetConnect();
                    foreach ((float dist, Dot d1, Dot d2) item in sortedDist)
                    {
                        if (ct.IsCancellationRequested) break;
                        if (item.d1.secondConnect == null && item.d2.secondConnect == null)
                        {
                            if (item.d1.startConnect == null && item.d2.startConnect == null)
                            {
                                connect(item, ct, step);
                            }
                            else
                            {
                                if (item.d1.startConnect == item.d2
                                && item.d2.startConnect == item.d1)
                                    connect(item, ct, step);
                            }
                        }
                    }
                });
        }

        public void SetCustomBuild2()
        {
            IEnumerable<(Dot dot, float coeff)> getCoeffs(Dot first, Dot current, Dot second, int count)
            {
                Point firstP = first.Point, currentP = current.Point, secondP = second.Point;
                LineVector vectorToPrev = LineVector.FindVector(secondP, currentP);
                LineVector vectorToFirst = LineVector.FindVector(secondP, firstP);
                IEnumerable<KeyValuePair<Dot, float>> fet = second.OtherDotDistances;
                if (count > -1)
                {
                    fet = fet.Where(a => a.Key.firstConnect is null) // Где нет соединения
                                .Where(a =>
                                    a.Value * 2.25 > vectorToPrev.Length    // Разница в растоянии не более,
                                    && a.Value < vectorToPrev.Length * 2.25);
                }
                else
                {
                    fet = fet.Where(a => a.Key.firstConnect is null)  // Где нет соединения
                    .Where(a =>
                        a.Value * 2 - count * 0.05f > vectorToPrev.Length    // Разница в растоянии не более,
                        && a.Value < vectorToPrev.Length * 2 - count * 0.05f);  // чем в 2 раза
                }
                IEnumerable<(Dot Key, float coeff)> fet2 = fet
                    //.OrderBy(a => a.Value)
                    //.Take(20)
                    .Select(a => (a.Key, Coeff(vectorToPrev, LineVector.FindVector(secondP, a.Key.Point))));
                if (first != current
                    && vectorToPrev.Length * 2 > vectorToFirst.Length
                    && vectorToPrev.Length < vectorToFirst.Length * 2) // Если возможно соединиться с начальной точкой
                {
                    var firstVectorToNext = LineVector.FindVector(first.Point, first.NextConnectDot.Point);
                    if (firstVectorToNext.Length * 2 > vectorToFirst.Length
                        && firstVectorToNext.Length < vectorToFirst.Length * 2)
                    {
                        //if (first == current)
                        //    fet2 = fet2.Where(a => a.Item2 <= 5).OrderBy(a => a.Item2).Concat(new[] { (first, 100f) });
                        //else
                        fet2 = fet2.Concat(new[] { (first, Coeff(vectorToPrev, vectorToFirst) * /*delta **/ 1f/*.1f*/) })
                            .Where(a => a.Item2 <= 5)
                            .OrderBy(a => a.Item2);
                    }
                    else fet2 = fet2.Where(a => a.Item2 < 5).OrderBy(a => a.Item2);
                }
                else fet2 = fet2.Where(a => a.Item2 < 5).OrderBy(a => a.Item2);
                return fet2;
                //return.Where(a => a.Item2 <= 4.5);
            }

            bool recoursive(List<Dot> dots, Dot first, Dot prev, Dot current, int count, CancellationToken ct = default, StepBase step = null)
            {
                Dot tmp = dots.First();
                if (dots.Count > 3)
                    foreach (var item in dots.Skip(1).SkipLast(1))
                    {
                        if (PointExtensions.AreCrossing(tmp.Point, item.Point, prev.Point, current.Point))
                        {
                            return false;
                        }
                        tmp = item;
                    }
                dots.Add(current);
                if (current.startConnect != null && (current.startConnect.firstConnect == null) || current == first)
                {
                    var dot = current.startConnect;
                    if (Coeff(
                        new LineVector
                        (
                            current.Point.X - prev.Point.X,
                            current.Point.Y - prev.Point.Y
                        ),
                        new LineVector
                        (
                            current.Point.X - dot.Point.X,
                            current.Point.Y - dot.Point.Y
                        )) > 50)
                    {
                        dots.Remove(current);
                        return false;
                    }
                    current.secondConnect = dot;
                    dot.firstConnect = current;
                    current.startConnect = null;
                    dot.startConnect = null;
                    step.Wait(); // Отладка.
                    if (dot == first)
                    {
                        return true;
                    }

                    if (recoursive(dots, first, current, dot, count + 1, ct, step))
                    {
                        return true;
                    }
                    else
                    {
                        current.secondConnect = null;
                        dot.firstConnect = null;
                        current.startConnect = dot;
                        dot.startConnect = current;
                        dots.Remove(current);

                        return false;
                    }
                }
                var fet = getCoeffs(first, prev, current, count);
                foreach (var (dot, coeff) in fet.Take(5))
                {
                    if (ct.IsCancellationRequested)
                    { // Задача отменена, замыкаем принудительно структуру для избежания ошибок
                        current.secondConnect = first;
                        first.firstConnect = current;
                        return true;
                    }
                    current.secondConnect = dot;
                    dot.firstConnect = current;
                    step.Wait(); // Отладка.

                    if (dot == first)
                    {
                        return true; // Если структура замкнулась, завершаем рекурсию
                    }
                    if (recoursive(dots, first, current, dot, count + 1, ct, step)) return true; // Если вложенный цикл вернул true, то структура уже замкнулась
                    dot.firstConnect = null;  // Если не нашлось соединений в внутреннем цикле, то отменяется последнее соединение
                    current.secondConnect = null;
                }
                dots.Remove(current);
                return false; // Не нашлось соединений
            }

            CasheDotDistances = true;
            ConnectCustom = (a, ct, step) =>
            {
                if (a.dist > 200) return false;
                Dot d1 = a.d1;
                Dot d2 = a.d2;
                Dot first = d1;
                Dot current = d1;
                Dot second = d2;
                //Point firstP = first.Point;
                //Point currentP = current.Point;
                //Point secondP = second.Point;
                int counter = 0;
                if (d1.firstConnect is null && d2.firstConnect is null)
                {
                    int len = second.OtherDotDistances
                        .Count(a => a.Key.firstConnect == null);
                    // Первоначальное соединение
                    current.firstConnect = current; // Временная заглушка, чтобы не показывалось в запросе
                    current.secondConnect = second;
                    second.firstConnect = current;
                    if (step == null) step = new StepBase();
                    step.Wait();
                    List<Dot> dots = new List<Dot>();
                    dots.Add(first);
                    if (recoursive(dots, first, current, second, 0, ct, step))
                    {
                        return true;
                    }
                    else
                    {
                        first = second;
                        second = current;
                        current = first;
                        current.firstConnect = current; // Временная заглушка, чтобы не показывалось в запросе
                        current.secondConnect = second;
                        second.firstConnect = current;
                        second.secondConnect = null;
                        //d2.firstConnect = d2;
                        //d2.secondConnect = d1;
                        //d1.firstConnect = d2;
                        //d1.secondConnect = null;
                        if (recoursive(dots, first, current, second, 0, ct, step))
                        {
                            return true;
                        }
                        else
                        {
                            d1.firstConnect = null;
                            d1.secondConnect = null;
                            d2.firstConnect = null;
                            d2.secondConnect = null;
                            return false;
                        }
                    }
                    //while (!ct.IsCancellationRequested)
                    //{
                    //    step.Wait();

                    //    //LineVector vectorToPrev = LineVector.FindVector(secondP, currentP);
                    //    //LineVector vectorToFirst = LineVector.FindVector(secondP, firstP);
                    //    //IEnumerable<(Dot dot, float coeff)> fet = second.OtherDotDistances
                    //    //    .Where(a => a.Key.firstConnect is null      // Где нет соединения
                    //    //        && a.Value * 5 > vectorToPrev.Length    // Разница в растоянии не более,
                    //    //        && a.Value < vectorToFirst.Length * 5)  // чем в 5 раз
                    //    //    .OrderBy(a => a.Value)
                    //    //    .Take(20)
                    //    //    .Select(a => (a.Key, Coeff(vectorToPrev, LineVector.FindVector(secondP, a.Key.Point))))
                    //    //    .Concat(new[] { (first, Coeff(vectorToPrev, vectorToFirst) * /*delta **/ (1.1f - counter * 0.1f)) });

                    //    //IEnumerable<(Dot dot, float coeff)> fet = getCoeffs(first, current, second);

                    //    //Dot dotMin = first;
                    //    //float min = vectorToPrev.Length * 2;
                    //    //foreach (var (dot, coeff) in fet)
                    //    //{
                    //    //    second.secondConnect = dot;
                    //    //    dot.firstConnect = second;

                    //    //    //if (coeff < min)
                    //    //    //{
                    //    //    //    dotMin = dot;
                    //    //    //    min = coeff;
                    //    //    //}
                    //    //}

                    //    //second.secondConnect = dotMin;
                    //    //dotMin.firstConnect = second;

                    //    //if (dotMin == first)
                    //    //{
                    //    //    step.Wait();
                    //    //    break;
                    //    //}

                    //    //current = second;
                    //    //second = dotMin;
                    //    ////currentP = secondP;
                    //    ////secondP = second.Point;
                    //    //counter++;
                    //}
                    return true;
                }
                else return false;
            };
        }

        public void SetCustomBuild1()
        {
            CasheDotDistances = true;
            ConnectCustom = (a, ct, step) =>
            {
                Dot d1 = a.d1;
                Dot d2 = a.d2;
                Dot first = d1;
                Dot current = d1;
                Dot second = d2;
                Point firstP = first.Point;
                Point currentP = current.Point;
                Point secondP = second.Point;
                int counter = 0;
                //if (d1.otherConnect2 == null && d2.otherConnect2 == null) return false;
                // Если еще не было соединений
                if (d1.firstConnect is null && d2.firstConnect is null)
                {
                    int len = second.OtherDotDistances
                        .Count(a => a.Key.firstConnect == null);
                    // Первоначальное соединение
                    d1.firstConnect = d1; // Временная заглушка, чтобы не показывалось в запросе
                    d1.secondConnect = d2;
                    d2.firstConnect = d1;
                    while (!ct.IsCancellationRequested)
                    {
                        step.Wait();
                        LineVector vectorToPrev = LineVector.FindVector(secondP, currentP);
                        LineVector vectorToFirst = LineVector.FindVector(secondP, firstP);
                        //float delta = vectorToFirst.Length / vectorToPrev.Length;
                        //if (delta < 1) delta = 1 / delta;
                        IEnumerable<(Dot dot, float coeff)> fet = second.OtherDotDistances
                            .Where(a => a.Key.firstConnect is null
                                && a.Value * 5 > vectorToPrev.Length
                                && a.Value < vectorToFirst.Length * 5)
                            .OrderBy(a => a.Value)
                            .Take(20)
                            .Select(a => (a.Key, Coeff(vectorToPrev, LineVector.FindVector(secondP, a.Key.Point))))
                            .Concat(new[] { (first, Coeff(vectorToPrev, vectorToFirst) * /*delta **/ (1.1f - counter * 0.1f)) });

                        Dot dotMin = first;
                        float min = vectorToPrev.Length * 2;
                        foreach (var (dot, coeff) in fet)
                        {
                            if (coeff < min)
                            {
                                dotMin = dot;
                                min = coeff;
                            }
                        }

                        second.secondConnect = dotMin;
                        dotMin.firstConnect = second;

                        if (dotMin == first)
                        {
                            step.Wait();
                            break;
                        }

                        current = second;
                        second = dotMin;
                        currentP = secondP;
                        secondP = second.Point;
                        counter++;
                    }
                    return true;
                }
                else return false;
                //else
                //{
                //    // Подходящие
                //    //Dictionary<Dot, float> success = new Dictionary<Dot, float>();
                //    Dot prev, first, second;
                //    if (d1.otherConnect1 is null)
                //    {
                //        prev = d2.otherConnect1;
                //        first = d2;
                //        second = d1;
                //    }
                //    else
                //    {
                //        prev = d1.otherConnect1;
                //        first = d1;
                //        second = d2;
                //    }

                //    float prevDistance = (float)prev.Point.Distance(first.Point);
                //    LineVector toPrev = LineVector.FindVector(first.Point, prev.Point);

                //    IEnumerable<(Dot dot, float dist)> fetch =
                //        first.OtherDotDistances
                //            .AsEnumerable()
                //            //.SkipWhile(a => a.Key != second)
                //            //.Skip(1)
                //            .Where(a => a.Key.otherConnect1 != first
                //                && a.Key.otherConnect2 is null
                //                && a.Key != prev)
                //            .Take(10) // Ограничитель
                //            .Select(a => (a.Key, Coeff(toPrev,
                //                LineVector.FindVector(first.Point, second.Point))));
                //    float min = float.MaxValue;
                //    Dot secondConnected = null;
                //    foreach (var item in fetch)
                //    {
                //        if (item.dist < min)
                //        {
                //            secondConnected = item.dot;
                //            min = item.dist;
                //        }
                //    }
                //    if (secondConnected is null) secondConnected = second;
                //    first.otherConnect2 = secondConnected;
                //    if (secondConnected.otherConnect1 is null) secondConnected.otherConnect1 = first;
                //    else secondConnected.otherConnect2 = first;
                //}
            };
            //ConnectInArrayCustom = arr =>
            //{
            //    var connect = GetConnect();
            //    foreach (var item in arr)
            //    {
            //        if (item.d1.firstConnect == null && item.d2.firstConnect == null)
            //        {
            //        }
            //        if (item.d1.secondConnect == null && item.d2.secondConnect == null)
            //        {
            //            connect(item);
            //        }
            //    }
            //};
            //CustomBuildLines = (dots, dist) =>
            //{
            //    void TryConnect(Dot d1, Dot d2)
            //    {
            //        const int delta = 1;
            //        // Если есть свободные места
            //        if (d1.otherConnect2 is null && d2.otherConnect2 is null)
            //        {
            //            // Если еще не было соединений
            //            if (d1.otherConnect1 is null && d2.otherConnect1 is null)
            //            {
            //                d1.otherConnect1 = d2;
            //                d2.otherConnect1 = d1;
            //            }
            //            else
            //            {
            //                // Подходящие
            //                //Dictionary<Dot, float> success = new Dictionary<Dot, float>();
            //                Dot prev, first, second;
            //                if (d1.otherConnect1 is null)
            //                {
            //                    prev = d2.otherConnect1;
            //                    first = d2;
            //                    second = d1;
            //                }
            //                else
            //                {
            //                    prev = d1.otherConnect1;
            //                    first = d1;
            //                    second = d2;
            //                }

            //                float prevDistance = (float)prev.Point.Distance(first.Point);
            //                LineVector toPrev = LineVector.FindVector(first.Point, prev.Point);

            //                IEnumerable<(Dot dot, float dist)> fetch =
            //                first.OtherDotDistances
            //                    .AsEnumerable()
            //                    .SkipWhile(a => a.Key != second)
            //                    .Where(a=>a.Key.otherConnect2 is null)
            //                    .Take(5) // Ограничитель
            //                    .Select(a =>
            //                        (a.Key,
            //                        // Разница расстояний по модулю
            //                        Math.Abs(a.Value - prevDistance) *
            //                        // * (Угол между векторами + 1)
            //                        (LineVector.FindVector(first.Point, a.Key.Point)
            //                            .FindAngle(toPrev) + delta)
            //                        ));
            //                float min = float.MaxValue;
            //                Dot secondConnected = null;
            //                foreach (var item in fetch)
            //                {
            //                    if (item.dist < min) secondConnected = item.dot;
            //                }

            //                first.otherConnect2 = secondConnected;
            //                if (secondConnected.otherConnect1 is null) secondConnected.otherConnect1 = first;
            //                else secondConnected.otherConnect2 = first;
            //            }
            //        }
            //    }

            //    foreach ((float dist, Dot d1, Dot d2) item in dist)
            //    {
            //        // Если нет вторых сохраненных соединений
            //        if (item.d1.otherConnect2 == null && item.d2.otherConnect2 == null)
            //        {
            //            TryConnect(item.d1, item.d2);
            //        }
            //    }
            //    var checkedLineSet = new HashSet<Dot>(new Dot.DotIDEqualityComparer());
            //    List<LineSet> lineSets = new List<LineSet>();
            //    int idCounter = 1;
            //    foreach (var item in points.Values)
            //    {
            //        if (item.LineSet != null) continue;

            //        List<Dot> dotConnections = new List<Dot>();
            //        Dot firstDot = item;
            //        Dot prevDot = item;
            //        Dot currentDot = item.otherConnect2;
            //        dotConnections.Add(firstDot);
            //        if (currentDot == null)
            //        {
            //            lineSets.Add(new LineSet(idCounter++, lineContainer, dotConnections.ToArray()));
            //            continue;
            //        }
            //        while (firstDot != currentDot)
            //        {
            //            if (prevDot == currentDot.otherConnect2)
            //            {
            //                Swap(ref currentDot.otherConnect1, ref currentDot.otherConnect2); // swap для сохранения порядка
            //            }
            //            dotConnections.Add(currentDot);
            //            prevDot = currentDot;
            //            currentDot = currentDot.otherConnect2;
            //        }
            //        lineSets.Add(new LineSet(idCounter++, lineContainer, dotConnections.ToArray()));
            //    }
            //    return lineSets;
            //};
        }

        public static float Coeff(LineVector toPrev, LineVector toNext)
        {
            float min = toPrev.Length, max = toNext.Length;
            if (min > max)
            {
                var t = min;
                min = max;
                max = t;
            }
            // Разница расстояний по модулю
            float distMod = (max / min);
            // * (Угол между векторами + 1)
            float angle = (float)Math.PI - toPrev.FindAngle(toNext);

            return ((distMod * 0.75f)) * (float)(Math.Pow(2, angle));
            //return (distMod * 0.75f) * (float)(angle + 3);
            //((float)Math.Exp((float)Math.PI - LineVector.FindVector(first.Point, a.Key.Point)
            //    .FindAngle(toPrev)) + (delta - 2));
        }

        public void BuildLineSets(CancellationToken cancellationToken = default, StepBase step = null, Func<SortedSet<Dot>> getSortetDots = null)
        {
            lock (__locker)
            {
                if (step == null) step = new StepBase();
                SortedSet<Dot> dots;
                if (getSortetDots == null)
                    dots = GetSortedDots();
                else dots = getSortetDots();
                Dot[] vs = dots.ToArray();
                // Очистка старых соединений
                foreach (var item in dots)
                {
                    item.ClearConnections();
                }
                lineContainer.Reset();
                lineSets.Clear();

                // Список расстояний между точками
                List<(float dist, Dot d1, Dot d2)> dist = new List<(float dist, Dot d1, Dot d2)>();

                if (CasheDotDistances)
                {
                    while (dots.Count > 0)
                    {
                        var dot = dots.First();
                        var id = dot.ID;
                        Dictionary<Dot, float> otherDotDistances = dot.otherDotDistances;
                        dots.Remove(dot);
                        foreach (var item in dots)
                        {
                            float distance = (float)dot.Point.Distance(item.Point);
                            item.otherDotDistances.Add(dot, distance);
                            otherDotDistances.Add(item, distance);
                            dist.Add((distance, dot, item));
                        }
                        dot.order = otherDotDistances.OrderBy(a => a.Value).Select(a => a.Key.ID).ToArray();
                    }
                }
                // Отсортированный массив расстояний между точками
                (float dist, Dot d1, Dot d2)[] sortedDist = dist.OrderBy(a => a.dist).ToArray();

                if (CustomBuildLines != null)
                {
                    var t = CustomBuildLines(vs, sortedDist);
                    //foreach (var item in t.lines)
                    //{
                    //    this.lineContainer.Add(item);
                    //}
                    foreach (var item in t)
                    {
                        this.lineSets.Add(item.ID, item);
                    }
                    return;
                }

                //var tryConnect = ConnectCustom;
                //if (tryConnect == null)
                //{
                //    tryConnect = item =>
                //    {
                //        if (item.d1.otherConnect1 == null) item.d1.otherConnect1 = item.d2;
                //        else item.d1.otherConnect2 = item.d2;
                //        if (item.d2.otherConnect1 == null) item.d2.otherConnect1 = item.d1;
                //        else item.d2.otherConnect2 = item.d1;
                //    };
                //}

                //var tryConnectInArray = ConnectInArrayCustom;
                //if (tryConnectInArray == null)
                //{
                //    tryConnectInArray = sortedDist =>
                //    {
                //        foreach ((float dist, Dot d1, Dot d2) item in sortedDist)
                //        {
                //            if (item.d1.otherConnect2 == null && item.d2.otherConnect2 == null)
                //            {
                //                tryConnect(item);
                //                //if (item.d1.otherConnect1 == null) item.d1.otherConnect1 = item.d2;
                //                //else item.d1.otherConnect2 = item.d2;
                //                //if (item.d2.otherConnect1 == null) item.d2.otherConnect1 = item.d1;
                //                //else item.d2.otherConnect2 = item.d1;
                //            }
                //        }
                //    };
                //}

                GetConnectInArray()(sortedDist, cancellationToken, step);
                var checkedLineSet = new HashSet<Dot>(new Dot.DotIDEqualityComparer());
                int idCounter = 1;
                foreach (var item in points.Values)
                {
                    if (item.LineSet != null) continue;

                    List<Dot> dotConnections = new List<Dot>();
                    Dot firstDot = item;
                    Dot prevDot = item;
                    Dot currentDot = item.secondConnect;
                    dotConnections.Add(firstDot);
                    if (currentDot == null)
                    {
                        lineSets.Add(idCounter, new LineSet(idCounter++, lineContainer, dotConnections.ToArray()));
                        continue;
                    }
                    while (firstDot != currentDot)
                    {
                        //try
                        //{
                        if (dotConnections.Count > 1000) break;// Debugger.Break();
                        if (prevDot == currentDot.secondConnect)
                        {
                            Swap(ref currentDot.firstConnect, ref currentDot.secondConnect); // swap для сохранения порядка
                        }
                        dotConnections.Add(currentDot);
                        prevDot = currentDot;
                        currentDot = currentDot.secondConnect;
                        //}
                        //catch { break; }
                    }
                    if (dotConnections.Count > 1)
                        lineSets.Add(idCounter, new LineSet(idCounter++, lineContainer, dotConnections.ToArray()));
                }
            }
        }

        public void FindRelations(CancellationToken cancellationToken = default, IEnumerable<LineSet> lineSets = null)
        {
            lock (__locker)
            {
                int counter = 0;
                List<(Line, Line)> intersected = new List<(Line, Line)>(); // Буфер пересечений линий
                List<LineSet> otherLineSets = new List<LineSet>();
                if (lineSets == null) lineSets = this.lineSets.Values;
                // lineSets - Множество многоугольников (a) [Все найденные многоугольники]
                // otherLineSets - Множество многоугольников (b) [Изначально пустое множество]
                foreach (var thisLineSet in lineSets) // Многоугольник (a)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    thisLineSet.Relations.Clear();
                    foreach (var otherLineSet in otherLineSets) // Другой многоугольник (b)
                    {
                        // В текущем двойной цикле не должны повторно сравниваться одни и те же многоугольники,
                        // т.к. изначально множество (b) пустое и оно заполняется элементами из множества (a)
                        // после завершения одной итерации из множества (a).
                        // Тем самым каждая фигура сравнивается с другой ровно один раз.
                        if (thisLineSet.RectangleF.IntersectsWith(otherLineSet.RectangleF)) // Если находятся рядом (пересечение Rectangle)
                        {
                            //int interCount = 0; // Сброс счетчика пересечений
                            List<(Line, Line)> localIntersected = new List<(Line, Line)>(); // локальный буфер пересечений линий
                            foreach (var thisLine in thisLineSet.lines) // Линия (a) многоугольника
                            {
                                if (thisLine.RectangleF.IntersectsWith(otherLineSet.RectangleF)) // Нахождение (a) линии рядом с (b) многоугольником
                                    foreach (var otherLine in otherLineSet.lines) // Линия (b) многоугольника
                                    {
                                        if (thisLine != null && otherLine != null) // ??
                                        {
                                            if (thisLine.IsIntersect(otherLine)) // Линия (a) пересекается с линией (b)
                                            {
                                                localIntersected.Add((thisLine, otherLine)); // Сохраняем пересечение
                                                                                             //intersected.Add((thisLine, otherLine));
                                                                                             //interCount++;
                                            }
                                        }
                                    }
                            }
                            if (localIntersected.Count > 0) // Если найдены пересечения
                            {
                                intersected.AddRange(localIntersected); // Заполнение общего буфера заполненых линий
                                                                        // Мапинг всех пересечений для многоугольников (a) и (b)

                                // Групировка и преобразование буфера пересечений
                                // Мапинг для многоугольника (a)
                                (Line Key, Line[] IntersectedLines)[] ps_a = localIntersected.GroupBy(a => a.Item1, a => a.Item2).
                                    Select(a => (a.Key, a.ToArray())).ToArray();
                                // Мапинг для многоугольника (b)
                                (Line Key, Line[] IntersectedLines)[] ps_b = localIntersected.GroupBy(a => a.Item2, a => a.Item1).
                                    Select(a => (a.Key, a.ToArray())).ToArray();

                                // Сохранение отношений
                                thisLineSet.Relations.Add(otherLineSet, new RelationInfo(RelationType.Interserct, ps_a));
                                otherLineSet.Relations.Add(thisLineSet, new RelationInfo(RelationType.Interserct, ps_b));
                            }
                            else // Если не было пересечений
                            {
                                // Достаточно проверить вложеность одной точки
                                if (thisLineSet.CheckContains(otherLineSet.dots[0].Point))
                                {
                                    // Сохранение отношений (a) внутри (b)
                                    thisLineSet.Relations.Add(otherLineSet,
                                        new RelationInfo(RelationType.Outer, new (Line, Line[])[0]));
                                    otherLineSet.Relations.Add(thisLineSet,
                                        new RelationInfo(RelationType.Inner, new (Line, Line[])[0]));
                                }
                                else if (otherLineSet.CheckContains(thisLineSet.dots[0].Point))
                                {
                                    // Сохранение отношений (b) внутри (a)
                                    thisLineSet.Relations.Add(otherLineSet,
                                        new RelationInfo(RelationType.Inner, new (Line, Line[])[0]));
                                    otherLineSet.Relations.Add(thisLineSet,
                                        new RelationInfo(RelationType.Outer, new (Line, Line[])[0]));
                                }
                            }
                        }
                    }
                    // Сохраняем многоугольник (a) во множество (b)
                    //this.lineSets.Add(thisLineSet.ID, thisLineSet);
                    otherLineSets.Add(thisLineSet);
                }

                var iss = intersected.Concat(intersected.Select(a => (a.Item2, a.Item1))).GroupBy(a => a.Item1).ToDictionary(a => a.Key, a
                    => a.Where(b => a.Key != b.Item2).Select(b => b.Item2).ToArray(), Line.LineIDEqualityComparer.Instance);
                this.LineIntersects = iss;
                Intersects = intersected.ToArray();
            }
        }

        public SortedSet<Dot> GetSortedDots()
        {
            return new SortedSet<Dot>(points.Values, Comparer<Dot>.Create((a, b) => a.ID.CompareTo(b.ID)));
        }

        public void BuildAndFindRelations(CancellationToken cancellationToken = default)
        {
            BuildLineSets(cancellationToken);
            FindRelations(cancellationToken);
        }

        public void Clear()
        {
            idCounter = 0;
            this.points.Clear();
            this.lineContainer.Clear();
            this.lineSets.Clear();
            this.Intersects = new (Line, Line)[0];
        }

        public void ClearStructs()
        {
            foreach (var item in this.points.Values)
            {
                item.Lines.Clear();
                item.firstConnect = null;
                item.secondConnect = null;
                item.LineSet = null;
            }
            this.lineContainer.Clear();
            this.lineSets.Clear();
            this.Intersects = Array.Empty<(Line, Line)>();
        }

        private static void Swap(ref Dot obj1, ref Dot obj2)
        {
            var t = obj1;
            obj1 = obj2;
            obj2 = t;
        }

        public Dot CreateDot(Point point)
        {
            Dot dot = new Dot(++idCounter, point);
            AddDot(dot);
            return dot;
        }

        public void RemoveDot(int id)
        {
            if (points.ContainsKey(id))
                points.Remove(id);
        }

        private void AddDot(Dot point)
        {
            points.Add(point.ID, point);
        }

        #endregion Methods

        #region Ctor

        public Map()
        {
            points = new SortedDictionary<int, Dot>();
            Dots = new ReadOnlyDictionary<int, Dot>(points);
            lineSets = new SortedDictionary<int, LineSet>();
            LineSets = new ReadOnlyDictionary<int, LineSet>(lineSets);
            lineContainer = new LineContainer(this);
            LineContainer = new ReadOnlyDictionary<int, Line>(lineContainer);

            Instance = this;
        }

        public Map(ICollection<Dot> dots) : this()
        {
            foreach (var item in dots)
            {
                AddDot(item);
            }
        }

        #endregion Ctor

        #region Events

        public event EventHandler<EventArgsDotChange> DotChanged;

        #endregion Events

        #endregion PublicMembers

        #region PrivateMembers

        private object __locker = new object();

        [NonSerialized]
        private int idCounter;

        [NonSerialized]
        private readonly IDictionary<int, Dot> points;

        [NonSerialized]
        private readonly IDictionary<int, LineSet> lineSets;

        [NonSerialized]
        private readonly LineContainer lineContainer;

        #endregion PrivateMembers
    }

    public class EventArgsDotChange : EventArgs
    {
        public int ID { get; }
        public Dot Dot { get; }
    }

    //public class EventArgsLineChange : EventArgs

    //public static class DrawClass
    //{
    //    public static void Draw()
    //    {
    //        SKImageInfo info = new SKImageInfo(640, 480, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
    //        using (var surface = SKSurface.Create (info))
    //        {
    //            SKCanvas canvas = surface.Canvas;
    //            // Your drawing code goes here.
    //            canvas.Clear (SKColors.White);
    //            // set up drawing tools
    //            using (var paint = new SKPaint ()) {
    //            paint.IsAntialias = true;
    //            paint.Color = new SKColor (0x2c, 0x3e, 0x50);
    //            paint.StrokeCap = SKStrokeCap.Round;
    //            // create the Xamagon path
    //            using (var path = new SKPath ())
    //            {
    //                path.MoveTo (71.4311121f, 56f);
    //                path.CubicTo (68.6763107f, 56.0058575f, 65.9796704f, 57.5737917f, 64.5928855f, 59.965729f);
    //                path.LineTo (43.0238921f, 97.5342563f);
    //                path.CubicTo (41.6587026f, 99.9325978f, 41.6587026f, 103.067402f, 43.0238921f, 105.465744f);
    //                path.LineTo (64.5928855f, 143.034271f);
    //                path.CubicTo (65.9798162f, 145.426228f, 68.6763107f, 146.994582f, 71.4311121f, 147f);
    //                path.LineTo (114.568946f, 147f);
    //                path.CubicTo (117.323748f, 146.994143f, 120.020241f, 145.426228f, 121.407172f, 143.034271f);
    //                path.LineTo (142.976161f, 105.465744f);
    //                path.CubicTo (144.34135f, 103.067402f, 144.341209f, 99.9325978f, 142.976161f, 97.5342563f);
    //                path.LineTo (121.407172f, 59.965729f);
    //                path.CubicTo (120.020241f, 57.5737917f, 117.323748f, 56.0054182f, 114.568946f, 56f);
    //                path.LineTo (71.4311121f, 56f);
    //                path.Close ();

    //                // draw the Xamagon path
    //                canvas.DrawPath (path, paint);
    //                }
    //            }
    //        }
    //    }
    //}
}