﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

using Point = System.Drawing.PointF;
using Utils.Attributes;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ModelBase
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    //[EditableProperty(nameof(Point), typeof(PointFConverter))]
    [Description("Точка, расположенная на карте")]
    [Expandable(true)]
    public class Dot
    {
        public class DotIDEqualityComparer : EqualityComparer<Dot>
        {
            public override bool Equals(Dot x, Dot y)
            {
                return x.ID.Equals(y.ID);
            }

            public override int GetHashCode(Dot obj)
            {
                return obj.ID;
            }
        }

        public ReadOnlyDictionary<Dot, float> OtherDotDistances { get; }
        internal readonly Dictionary<Dot, float> otherDotDistances = new Dictionary<Dot, float>();
        internal int[] order;

        [DisplayName("Объект")]
        public LineSet LineSet { get; internal set; }

        public int IndexOnLineSet { get; internal set; }
        public Dot firstConnect;
        public Dot secondConnect;

        public Dot? startConnect;


        [ReadOnly(true)]
        public List<Line> Lines { get; set; } = new List<Line>();

        //public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        //public event PropertyChangedEventHandler PropertyChanged;

        [DisplayName("Предыдущее соединение")]
        [ReadOnly(true)]
        public Dot PrevConnectDot => firstConnect;

        [DisplayName("Следующее соединение")]
        [ReadOnly(true)]
        public Dot NextConnectDot => secondConnect;

        public Dot(int id, Point point) : this()
        {
            Point = point;
            ID = id;
        }

        private Dot()
        {
            OtherDotDistances = new ReadOnlyDictionary<Dot, float>(otherDotDistances);
        }

        public int ID { get; }
        private Point point;

        [DisplayName("Координата")]
        [Description("Расположение точки на карте")]
        public Point Point
        {
            get => point;
            set
            {
                point = value;
                PointChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override string ToString()
        {
            string str = $"ID-{ID} Point-{Point}";
            if (firstConnect != null) str += $" con1-{firstConnect.ID})";
            if (secondConnect != null) str += $" con2-{secondConnect.ID})";
            return str;
        }

        internal void ClearConnections()
        {
            firstConnect = null;
            secondConnect = null;
            otherDotDistances.Clear();
            Lines.Clear();
            LineSet = null;
        }

        public event EventHandler PointChanged;
    }

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