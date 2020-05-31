using ModelBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModelView
{
    public static class GraphicsExtensions
    {
        public static Graphics Zoom(this Graphics g, float zoomScale)
        {
            using (Matrix transform = g.Transform)
            {
                if (zoomScale != 1.0)
                    transform.Scale(zoomScale, zoomScale, MatrixOrder.Append);
                g.Transform = transform;
            }
            return g;
        }

        //public static Graphics Offset(this Graphics g, Point offset)
        //{
        //    if (!offset.IsEmpty)
        //        using (Matrix transform = g.Transform)
        //        {
        //            transform.Translate(offset.X, offset.Y);
        //            g.Transform = transform;
        //        }
        //    return g;
        //}
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MapVisualizer : INotifyPropertyChanged
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Image background;
        private Map map;
        private float zoomScale = 1;
        private int widthLimit;
        private int heigthLimit;
        private float offsetX;
        private float offsetY;
        private Dot selectedDot;
        private Line selectedLine;
        private LineSet selectedLineSet;
        private Line[] intersectedLines;
        private int dotRadius = 6;

        private void OnPropertyChanged(bool reDraw, [CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (reDraw) DrawRefreshed?.Invoke(this, EventArgs.Empty);
        }

        public Image Background
        {
            get => background;
            set
            {
                background = value;
                OnPropertyChanged(true);
            }
        }

        public Map Map
        {
            get => map;
            set
            {
                map = value;
                OnPropertyChanged(true);
            }
        }

        public float ZoomScale
        {
            get => zoomScale;
            set
            {
                zoomScale = value;
                OnPropertyChanged(true);
            }
        }

        public int WidthLimit
        {
            get => widthLimit;
            set
            {
                widthLimit = value;
                OnPropertyChanged(false);
            }
        }

        public int HeigthLimit
        {
            get => heigthLimit;
            set
            {
                heigthLimit = value;
                OnPropertyChanged(false);
            }
        }

        public float OffsetX
        {
            get => offsetX;
            set
            {
                offsetX = value;
                OnPropertyChanged(true);
            }
        }

        public float OffsetY
        {
            get => offsetY;
            set
            {
                offsetY = value;
                OnPropertyChanged(true);
            }
        }

        public Dot SelectedDot
        {
            get => selectedDot;
            set
            {
                selectedDot = value;
                OnPropertyChanged(true);
            }
        }

        public Line SelectedLine
        {
            get => selectedLine;
            set
            {
                selectedLine = value;
                OnPropertyChanged(true);
            }
        }

        public LineSet SelectedLineSet
        {
            get => selectedLineSet; set
            {
                selectedLineSet = value;
                OnPropertyChanged(true);
            }
        }

        public Line[] IntersectedLines
        {
            get => intersectedLines; set
            {
                intersectedLines = value;
                OnPropertyChanged(true);
            }
        }

        public int DotRadius
        {
            get => dotRadius;
            set
            {
                if (value <= 0) throw new InvalidOperationException();
                dotRadius = value;
                OnPropertyChanged(true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler DrawRefreshed;

        #region TranslateMethods

        #region Rectangle

        public RectangleF TranslateRectangle(Rectangle rectangle)
        {
            return new RectangleF(TranslatePoint(rectangle.Location), TranslateSize(rectangle.Size));
        }

        public Rectangle TranslateRectangleReverse(RectangleF rectangle)
        {
            return new Rectangle(TranslatePointReverse(rectangle.Location), TranslateSizeReverse(rectangle.Size));
        }

        public RectangleF TranslateRectangleFReverse(RectangleF rectangle)
        {
            return new RectangleF(TranslatePointFReverse(rectangle.Location), TranslateSizeFReverse(rectangle.Size));
        }

        #endregion Rectangle

        #region Size

        public SizeF TranslateSize(Size size)
        {
            var zoom = (float)ZoomScale;
            return new SizeF(size.Width * zoom, size.Height * zoom);
        }

        public Size TranslateSizeReverse(SizeF size)
        {
            var zoom = (float)ZoomScale;
            return new Size((int)(size.Width * zoom), (int)(size.Height * zoom));
        }

        public SizeF TranslateSizeFReverse(SizeF size)
        {
            var zoom = (float)ZoomScale;
            return new SizeF(size.Width * zoom, size.Height * zoom);
        }

        #endregion Size

        #region Point

        public Point TranslatePointReverse(PointF point)
        {
            var zoom = ZoomScale;
            return new Point((int)((point.X /*- OffsetX*/) * zoom), (int)((point.Y/* - OffsetY*/) * zoom));
        }

        public PointF TranslatePointFReverse(PointF point)
        {
            var zoom = ZoomScale;
            return new PointF((float)((point.X) * zoom /*- OffsetX*/), (float)((point.Y /*- OffsetY*/) * zoom /*- OffsetY*/));
        }

        public PointF TranslatePoint(Point point)
        {
            var zoom = ZoomScale;
            return new PointF(/*OffsetX +*/ (float)(point.X / zoom), /*OffsetY +*/ (float)(point.Y / zoom));
        }

        public PointF TranslatePoint(PointF pointF)
        {
            var zoom = ZoomScale;
            return new PointF(/*OffsetX +*/ (float)(pointF.X / zoom), /*OffsetY +*/ (float)(pointF.Y / zoom));
        }

        #endregion Point

        #endregion TranslateMethods

        #region Bools

        public bool DrawRectangle { get; set; }
        public bool DrawLine { get; set; }
        public bool DrawIntersectLine { get; set; }

        #endregion Bools

        #region Pens

        public Pen PenRectangle { get; set; } = new Pen(Color.Gray, 2);
        public Pen InterPenRectangle { get; set; } = new Pen(Color.Green, 2);
        public Pen PenLine { get; set; }
        public Pen PenIntersectLine { get; set; } = new Pen(Color.Red, 4);

        public Pen[] PenColections { get; set; } = new Pen[]
        {
            Pens.Black,
            //Pens.Yellow,
            //Pens.AliceBlue,
            Pens.Green,
            //Pens.Aqua,
            Pens.Magenta,
            Pens.RosyBrown,
            Pens.Silver,
            Pens.Tomato,
            Pens.Maroon
        };

        public Color[] CollorCollections { get; set; } = new Color[]
        {
           Color.Black,
           Color.Green,
           Color.Magenta,
           Color.RosyBrown,
           Color.Silver,
           Color.Tomato,
           Color.Maroon
        };

        #endregion Pens

        public void Draw(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            if (Background != null)
            {
                g.TranslateTransform(offsetX, offsetY);
                g.ScaleTransform(zoomScale, zoomScale);
                g.DrawImage(Background, Point.Empty);
                //g.Offset(new Point(-offsetX, -offsetY)).Zoom((float)zoomScale).DrawImage(Background, Point.Empty);
                g.ResetTransform();
            }
            if (Map != null)
            {
                g.TranslateTransform(offsetX * zoomScale, offsetY * zoomScale);
                foreach (var item in Map.LineSets.Values)
                {
                    //if (item.Selected)
                    //    g.DrawRectangle(PenRectangle, TranslateRectangleReverse(item.RectangleF));
                    if (!item.Display) continue;
                    //var oldClip = g.Clip;
                    if (Map.FillRelations)
                    {
                        List<PointF[]> exclude = new List<PointF[]>();
                        foreach (var item2 in item.Relations)
                        {
                            if (item2.Value.RelationType == RelationType.Outer)
                            {
                                exclude.Add(item2.Key.dots.Select(a => a.Point).ToArray());
                                //using (Brush brush = new HatchBrush(HatchStyle.WideUpwardDiagonal, item2.Key.Color, item.Color))
                                //{
                                //    GraphicsPath gp = new GraphicsPath();
                                //    gp.StartFigure();
                                //    gp.AddLines(item.dots.Select(a => a.Point).ToArray());
                                //    gp.CloseFigure();
                                //    Region rg = new Region();
                                //    rg.Intersect(gp);
                                //    gp = new GraphicsPath();
                                //    gp.StartFigure();
                                //    gp.AddLines(item2.Key.dots.Select(a => a.Point).ToArray());
                                //    gp.CloseFigure();
                                //    rg.Exclude(gp);
                                //    //g.Clip = rg;
                                //    g.FillRegion(brush, rg);
                                //}
                            }
                        }

                        if (exclude.Count > 0)
                        {
                            using (Brush brush = new HatchBrush(HatchStyle.DiagonalBrick, item.Color, Color.Transparent))
                            {
                                Region rg = new Region();
                                GraphicsPath gp = new GraphicsPath();
                                gp.StartFigure();
                                gp.AddLines(item.dots.Select(a => a.Point).ToArray());
                                gp.CloseFigure();
                                rg.Intersect(gp);
                                foreach (var item2 in exclude)
                                {
                                    gp = new GraphicsPath();
                                    gp.StartFigure();
                                    gp.AddLines(item2);
                                    gp.CloseFigure();
                                    rg.Exclude(gp);
                                }
                                //g.Clip = rg;
                                using (Matrix transform = new Matrix())
                                {
                                    transform.Scale(zoomScale, zoomScale, MatrixOrder.Append);
                                    //transform.Translate(offsetX, offsetY);
                                    rg.Transform(transform);
                                }
                                g.FillRegion(brush, rg);
                                rg.Dispose();
                                gp.Dispose();
                            }
                        }
                    }
                    using (Pen pen = new Pen(item.Color, 2))
                    {
                        foreach (var item2 in item.lines)
                        {
                            g.DrawLine(pen,
                                TranslatePointFReverse(item2.First.Point),
                                TranslatePointFReverse(item2.Second.Point));
                        }
                    }
                }

                foreach (var item in Map.LineContainer.Values)
                {
                }

                if (Map.LineSets.Count == 0) // Режим отладки
                {
                    using Pen pen = new Pen(Color.DarkRed, 3);
                    foreach (var item in Map.Dots.Values)
                    {
                        if (item.NextConnectDot != null)
                            g.DrawLine(pen,
                                  TranslatePointFReverse(item.Point),
                                  TranslatePointFReverse(item.NextConnectDot.Point));
                    }
                }

                var penIntersectLine = PenIntersectLine;
                foreach (var item in Map.Intersects)
                {
                    g.DrawLine(penIntersectLine,
                        TranslatePointReverse(item.Item1.First.Point),
                        TranslatePointReverse(item.Item1.Second.Point));
                    g.DrawLine(penIntersectLine,
                        TranslatePointReverse(item.Item2.First.Point),
                        TranslatePointReverse(item.Item2.Second.Point));
                }

                var rad = DotRadius;

                //try
                //{
                if (SelectedDot != null)
                {
                    using Font font = new Font(FontFamily.GenericSansSerif, 10);
                    Brush br = Brushes.Black;
                    if (SelectedDot.PrevConnectDot != null)
                    {
                        var prevVector = LineVector.FindVector(SelectedDot.Point, SelectedDot.PrevConnectDot.Point);
                        foreach (var item in SelectedDot.OtherDotDistances
                            .Where(a => a.Key != SelectedDot.PrevConnectDot)
                            .OrderBy(a => a.Value)
                            .Take(20))
                        {
                            var dot = item.Key;

                            ControlPaint.DrawRadioButton(g, TranslatePointFReverse(dot.Point).GetRadiusRectangle(rad + 5).ToRectangle(),
                            ButtonState.Inactive);
                            var vector = LineVector.FindVector(SelectedDot.Point, dot.Point);
                            //if(vector.Length / prevVector.Length < 2f)
                            //{
                            g.DrawString($"angle:{Math.PI - prevVector.FindAngle(vector):0.###}" +
                                $"{Environment.NewLine}len:{vector.Length:0.###}" +
                                $"{Environment.NewLine}coeff:{Map.Coeff(prevVector, vector):0.###}", font, br, TranslatePointFReverse(dot.Point));
                            //}
                        }

                        ControlPaint.DrawRadioButton(g, TranslatePointFReverse(SelectedDot.Point).GetRadiusRectangle(rad + 5).ToRectangle(),
                            ButtonState.Flat);
                        // Предыдущая точка
                        g.DrawString($"(prev) len:{prevVector.Length:0.###}" +
                            $"{Environment.NewLine}coeff:{Map.Coeff(prevVector, prevVector):0.###}", font, br, TranslatePointFReverse(SelectedDot.PrevConnectDot.Point));
                    }
                    if (SelectedDot.LineSet != null)
                    {
                        var p = new PointF(
                        SelectedDot.LineSet.dots.First().Point.X,
                        SelectedDot.LineSet.dots.First().Point.Y - 15
                            );
                        g.DrawString("First", font, br, TranslatePointFReverse(p));
                    }
                }
                //}
                //catch
                //{
                //}
                if (SelectedLineSet != null)
                {
                    g.DrawRectangle(PenRectangle, TranslateRectangleReverse(SelectedLineSet.RectangleF));
                }
                foreach (var item in Map.Dots)
                {
                    ControlPaint.DrawRadioButton(g, TranslatePointFReverse(item.Value.Point).GetRadiusRectangle(rad).ToRectangle(),
                        ButtonState.Flat | ButtonState.Checked);
                }
                if (SelectedLine != null)
                {
                    g.DrawRectangle(PenRectangle, TranslateRectangleReverse(SelectedLine.RectangleF));
                    g.DrawLine(new Pen(Color.Gray, 3), TranslatePointReverse(SelectedLine.First.Point), TranslatePointReverse(SelectedLine.Second.Point));
                }
                if (IntersectedLines != null)
                {
                    foreach (var item2 in IntersectedLines)
                    {
                        g.DrawRectangle(InterPenRectangle, TranslateRectangleReverse(item2.RectangleF));
                        g.DrawLine(InterPenRectangle, TranslatePointReverse(item2.First.Point), TranslatePointReverse(item2.Second.Point));
                    }
                }

                //if(DebugDot != null)
                //{
                //    ControlPaint.DrawRadioButton(g, TranslatePointFReverse(DebugDot.PrevConnectDot.Point).GetRadiusRectangle(rad + 5).ToRectangle(),
                //        ButtonState.Flat);

                //}
            }
        }

        public Dot FindDotFromTranslatedPoint(PointF point)
        {
            float r = (float)(DotRadius / ZoomScale);
            foreach (var item in Map.Dots.Values)
            {
                if (item.Point.GetRadiusRectangle(r).Contains(point))
                {
                    return item;
                }
            }
            return null;
        }

        public Dot FindDot(Point point)
        {
            PointF pointf = TranslatePoint(point);
            return FindDotFromTranslatedPoint(pointf);
        }

        public IEnumerable<Dot> FindDots(Point point)
        {
            PointF pointf = TranslatePoint(point);
            float r = (float)(DotRadius / ZoomScale);
            foreach (var item in Map.Dots.Values)
            {
                if (item.Point.GetRadiusRectangle(r).Contains(pointf))
                {
                    yield return item;
                }
            }
        }
    }
}