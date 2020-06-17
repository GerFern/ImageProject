using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Visuals.Media.Imaging;
using ImageLib.Image;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Brush = Avalonia.Media.Brush;
using Pen = Avalonia.Media.Pen;

namespace ImageLib.Model.Drawing
{
    public class DrawModel : ReactiveObject
    {
        public AvaloniaList<DrawModel> Draws { get; } = new AvaloniaList<DrawModel>();

        public virtual void StartDraw(DrawingContext drawingContext)
            => DrawChilds(drawingContext);

        protected void DrawChilds(DrawingContext drawingContext)
        {
            foreach (var item in Draws)
                item.StartDraw(drawingContext);
        }
    }

    public class MatrixImageModel : DrawModel
    {
        [Reactive] public IMatrixImage Image { get; set; }
        [Reactive] public double Opacity { get; set; }
        [Reactive] public Rect SourceRect { get; set; }
        [Reactive] public Rect DestRect { get; set; }

        [Reactive]
        public BitmapInterpolationMode BitmapInterpolationMode { get; set; }
            = BitmapInterpolationMode.Default;

        [Reactive] public bool DrawMatrixImageModels { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            drawingContext.DrawImage(Image.CreateBitmap(), Opacity,
                SourceRect, DestRect, BitmapInterpolationMode);
            if (DrawMatrixImageModels)
            {
                foreach (var item in Image.DrawModels)
                    item.StartDraw(drawingContext);
            }
            base.StartDraw(drawingContext);
        }
    }

    public class LineModel : DrawModel
    {
        [Reactive] private Pen Pen { get; set; }
        [Reactive] private Avalonia.Point Point1 { get; set; }
        [Reactive] private Avalonia.Point Point2 { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            drawingContext.DrawLine(Pen, Point1, Point2);
            base.StartDraw(drawingContext);
        }

        public LineModel(Pen pen, Avalonia.Point start, Avalonia.Point end)
        {
            Pen = pen;
            Point1 = start;
            Point2 = end;
        }
    }

    public class RectangleModel : DrawModel
    {
        [Reactive] public Pen Pen { get; set; }
        [Reactive] public Rect Rect { get; set; }
        [Reactive] public int CornerRadius { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Pen, Rect, CornerRadius);
            base.StartDraw(drawingContext);
        }

        public RectangleModel()
        {
        }

        public RectangleModel(Pen pen, Rect rect, int cornerRadius = 0)
        {
            this.Pen = pen;
            this.Rect = rect;
        }
    }

    public class FillRectangleModel : DrawModel
    {
        [Reactive] public Brush Brush { get; set; }
        [Reactive] public Rect Rect { get; set; }
        [Reactive] public int CornerRadius { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            drawingContext.FillRectangle(Brush, Rect, CornerRadius);
            base.StartDraw(drawingContext);
        }
    }

    public class PushGeometryClipModel : DrawModel
    {
        [Reactive] public Geometry Geometry { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            using (drawingContext.PushGeometryClip(Geometry))
            {
                base.StartDraw(drawingContext);
            }
        }
    }

    public class PushOpacityModel : DrawModel
    {
        [Reactive] public double Opacity { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            using (drawingContext.PushOpacity(Opacity))
            {
                base.StartDraw(drawingContext);
            }
        }
    }

    public class PushOpacityMaskModel : DrawModel
    {
        [Reactive] public Brush Brush { get; set; }
        [Reactive] public Rect Rect { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            using (drawingContext.PushOpacityMask(Brush, Rect))
            {
                base.StartDraw(drawingContext);
            }
        }
    }

    public class PushPreTransformModel : DrawModel
    {
        [Reactive] public Matrix Matrix { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            using (drawingContext.PushPreTransform(Matrix))
            {
                base.StartDraw(drawingContext);
            }
        }
    }

    public class PushPostTransformModel : DrawModel
    {
        [Reactive] public Matrix Matrix { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            using (drawingContext.PushPostTransform(Matrix))
            {
                base.StartDraw(drawingContext);
            }
        }
    }

    public class DrawTextModel : DrawModel
    {
        [Reactive] public Brush Brush { get; set; }
        [Reactive] public Avalonia.Point Point { get; set; }
        [Reactive] public string Text { get; set; }
        [Reactive] public Typeface Typeface { get; set; }
        [Reactive] public TextAlignment TextAlignment { get; set; }
        [Reactive] public Avalonia.Size Constraint { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            var text = new FormattedText()
            {
                Text = Text,
                Typeface = Typeface,
                TextAlignment = TextAlignment,
                Constraint = Constraint,
            };

            drawingContext.DrawText(Brush, Point, text);
            base.StartDraw(drawingContext);
        }
    }

    public class PushTransformContainerModel : DrawModel
    {
        public override void StartDraw(DrawingContext drawingContext)
        {
            using (drawingContext.PushTransformContainer())
            {
                base.StartDraw(drawingContext);
            }
        }
    }

    public class GeometryModel : DrawModel
    {
        [Reactive] private Pen Pen { get; set; }
        [Reactive] private Brush Brush { get; set; }

        [Reactive] private Geometry Geometry { get; set; }

        public override void StartDraw(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(Brush, Pen, Geometry);
            base.StartDraw(drawingContext);
        }

        public GeometryModel()
        {
        }

        public GeometryModel(Geometry geometry, Pen pen, Brush brush)
        {
            Geometry = geometry;
            Pen = pen;
            Brush = brush;
        }
    }
}