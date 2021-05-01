using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Utils;
using Utils.Attributes;
using Utils.Converters;

namespace ModelBase
{
    /// <summary>
    /// Набор линий. Отдельная структура
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LineSet
    {
        /// <summary>
        /// Сравнивание по ID, чтобы не приходилось сравнивать по ссылкам
        /// </summary>
        public class LineSetEqualityComparer : IEqualityComparer<LineSet>
        {
            public static LineSetEqualityComparer Instance { get; } = new LineSetEqualityComparer();

            bool IEqualityComparer<LineSet>.Equals(LineSet x, LineSet y) => x.ID.Equals(y.ID);

            int IEqualityComparer<LineSet>.GetHashCode(LineSet obj) => obj.ID.GetHashCode();
        }

        [Browsable(false)]
        public Map Map { get; set; }

        [Browsable(false)]
        public int ID { get; }

        public Dot[] dots;
        public Line[] lines;
        public RectangleF[] dotRectangles;
        public float[] dotDistances;
        public RelationLineSets IntersectLineSets;
        public RectangleF RectangleF;

        [DisplayName("Напрваление отрезка")]
        public Direction Direction { get; }

        public Dot[] Dots => dots;

        [Browsable(false)]
        public bool Selected { get; set; } = false;

        [DisplayName("Видимость")]
        [Description("Видимость объекта на карте")]
        public bool Display { get; set; } = true;

        [DisplayName("Цвет")]
        [Description("Цвет линий, отображаемых на карте")]
        public Color Color { get; set; }

        [DisplayName("Отношения")]
        [Description("Отношения с другими фигурами")]
        [Expandable]
        [KeyProperty(nameof(LineSet.ID))]
        [EditableProperty(nameof(RelationInfo.RelationType), typeof(RelationType), typeof(DescriptionEnumConverter))]
        [TypeConverter(typeof(ReadOnlyDictionaryTypeConverter))]
        /// <summary>
        /// Отношения с другими фигурами
        /// </summary>
        public Dictionary<LineSet, RelationInfo> Relations { get; } =
            new Dictionary<LineSet, RelationInfo>(LineSetEqualityComparer.Instance);

        /// <summary>
        /// Расположение точки внутри многоуголника
        /// </summary>
        /// <param name="pointF"></param>
        /// <returns></returns>
        public bool CheckContains(PointF pointF)
        {
            int count = 0;
            PointF first = lines[0].First.Point;
            float x = pointF.X, y = pointF.Y;
            float curX, curY, prevX, prevY;
            prevX = first.X;
            prevY = first.Y;
            foreach (var item in lines)
            {
                PointF second = item.Second.Point;
                curX = second.X;
                curY = second.Y;

                if ((((curY < y && y < prevY) || (prevY < y && y < curY)) // Сравнение по высоте
                       && (y - curY) / (prevY - curY) * (prevX - curX) < x - curX) // Точка находится правее отрезка
                       || (item.Direction != Direction.None && curY == y && curX < x))
                    // Или высота второй точки совпадает и точка находится справа
                    count++; // Увеличиваем счетчик на 1

                prevX = curX;
                prevY = curY;
            }

            return count % 2 == 1;
        }

        /// <summary>
        /// Определение условия нахождения точки внутри произвольного многоугольника
        /// </summary>
        /// <param name="vs">Точки, описывающие многоугольник</param>
        /// <param name="point">Целевая точка</param>
        /// <returns></returns>
        public static bool CheckContains(IEnumerable<PointF> vs, PointF point)
        {
            int count = 0;
            PointF first = vs.First();
            float x = point.X, y = point.Y;
            float curX, curY, prevX, prevY;
            prevX = first.X; prevY = first.Y;

            foreach (var item in vs.Skip(1).Concat(new PointF[] { first }))
            {
                curX = item.X;
                curY = item.Y;

                if ((curY < y && y < prevY || prevY < y && y < curY) && // Совпадение по высоте
                    (y - curY) / (prevY - curY) * (prevX - curX) < x - curX) // Точка находится правее отрезка
                    count++; // Увеличиваем счетчик на 1

                prevX = curX;
                prevY = curY;
            }

            System.Diagnostics.Debug.WriteLine($"{count % 2 == 1} + {count}");
            return count % 2 == 1; // Если с одной стороны было нечетное кол-во линий, то точка находится внутри многоугольника
                                   //for (int i = 0; i < len; i++) // Подсчет линий
                                   //{
                                   //    PointF pCur = vs[i];
                                   //    PointF pPrev = vs[j];
                                   //    if ((pCur.Y < point.Y && pPrev.Y >= point.Y || pPrev.Y < point.Y && pCur.Y >= point.Y) &&
                                   //        pCur.X + (point.Y - pCur.Y) / (pPrev.Y - pCur.Y) * (pPrev.X - pCur.X) < point.X)
                                   //        res = !res;
                                   //    j = i;
                                   //}
        }

        ///// <summary>
        ///// key - Пересекаемый набор линий (другой объект)<br/>
        ///// value - Список пересекаемых линий (линия данного объекта, линии другого объекта)
        ///// </summary>
        public void CheckIntersects(LineSet otherLineSet)
        {
            var otherIntersects = otherLineSet.IntersectLineSets;
            IntersectLines thisIntersectLines = new IntersectLines();
            IntersectLines otherIntersectLines = new IntersectLines();
            foreach (var line in lines)
            {
                List<Line> lineInters = new List<Line>();
                foreach (var otherLine in otherLineSet.lines)
                {
                    if (line.IsIntersect(otherLine))
                    {
                        lineInters.Add(otherLine);
                        //intersects.Add(otherItem); // В набор текущего объекта пересечений добавляем линию из другого набора
                        //var otherIntersects = otherLineSet.intersects; // Набор пересечений другого объекта
                        //List<Line> otherIntersectsValues;
                        //if (otherIntersects.ContainsKey(this))
                        //{
                        //    otherIntersectsValues = otherIntersects[this];
                        //}
                        //else
                        //{
                        //    otherIntersectsValues = new List<Line>();
                        //    otherIntersects.Add(this, otherIntersectsValues);
                        //}
                        //otherIntersectsValues.Add(item); // Добавление записи о пересечении для другого объекта
                    }
                }
                if (lineInters.Count > 0)
                {
                }
            }
        }

        public LineSet(int id, LineContainer container, IEnumerable<Dot> dots)
        {
            ID = id;
            this.dots = dots.ToArray();
            //if(dots.Last().NextConnectDot)
            foreach (var item in this.dots)
            {
                item.LineSet = this;
                item.PointChanged += DotPointChanged;
            }
            List<Dot> pl = new List<Dot>();
            Dot first, prev;
            prev = first = this.dots.First();
            PointF firstP, prevP = prev.Point;
            firstP = first.Point;
            float minX, maxX, minY, maxY;
            minX = maxX = firstP.X;
            minY = maxY = firstP.Y;
            //PointF first = dots.First().Point;
            //PointF prev = first;
            int counter = 0;
            int length = this.dots.Length;
            dotDistances = new float[length];
            dotRectangles = new RectangleF[length];
            lines = new Line[length];
            Line line;
            foreach (var item in this.dots.Skip(1))
            {
                PointF currentP = item.Point;
                //dotRectangles[counter] = currentP.GetRectangleFromTwoPoint(prevP);
                dotDistances[counter] = (float)currentP.Distance(prevP);
                line = new Line(prev, item, this, container);
                lines[counter] = line;
                dotRectangles[counter] = line.RectangleF;
                prev = item;
                prevP = prev.Point;
                counter++;

                float x = currentP.X, y = currentP.Y;
                if (x < minX) minX = x;
                else if (x > maxX) maxX = x;

                if (y < minY) minY = y;
                else if (y > maxY) maxY = y;
            }
            dotDistances[counter] = (float)firstP.Distance(prevP);
            line = new Line(prev, first, this, container);
            lines[counter] = line;
            dotRectangles[counter] = line.RectangleF;

            RectangleF = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            Color = ColorCollections[ID % ColorCollections.Length];
        }

        public LineSet(Map mapOwner, int id, LineContainer container, Dot[] dots) :
            this(id, container, dots)
        {
            Map = mapOwner;
        }

        private void DotPointChanged(object sender, EventArgs e)
        {
            PointF point = this.dots[0].Point;
            float minX, maxX, minY, maxY;
            minX = maxX = point.X;
            minY = maxY = point.Y;
            float x, y;
            foreach (var item in this.dots.Skip(1))
            {
                point = item.Point;
                x = point.X;
                y = point.Y;
                if (minX > x) minX = x;
                else if (maxX < x) maxX = x;
                if (minY > y) minY = y;
                else if (maxY < y) maxY = y;
            }
            RectangleF = new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        ~LineSet()
        {
            foreach (var item in this.dots)
            {
                item.PointChanged -= DotPointChanged;
            }
        }

        public override string ToString()
        {
            return $"{RectangleF}; Отношений {Relations.Count}";
        }

        public static Color[] ColorCollections { get; set; } = new Color[]
        {
        Color.Black,
        Color.Green,
        Color.Firebrick,
        Color.RosyBrown,
        Color.Silver,
        Color.Tomato,
        Color.Maroon
        };
    }

    [TypeConverter(typeof(DescriptionEnumConverter))]
    public enum Direction
    {
        [Description("Нет")]
        None,

        [Description("Вверх")]
        Up,

        [Description("Вниз")]
        Down
    }
}