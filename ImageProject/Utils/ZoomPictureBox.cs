using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;

namespace ImageProject.Utils
{
    public static class ZoomableGraphics
    {
        public static void MouseWheelSetZoomScale<TControl>(this TControl control, MouseEventArgs e, double zoomDelta) where TControl : Control, IZoomable
        {
            double scale = 1.0;
            if (e.Delta > 0)
            {
                scale = zoomDelta;
            }
            else if (e.Delta < 0)
            {
                scale = 1/zoomDelta;
            }
            else
                return;

            control.SetZoomScale(control.ZoomScale * scale, e.Location);
        }

        

        public static void SetScrollBarVisibilityAndMaxMin<TControl>(this TControl control, Size contentSize, Size clientSize) where TControl:Control,IZoomable
        {
            #region determine if the scroll bar should be visible or not
            HScrollBar horizontalScrollBar = control.HorizontalScrollBar;
            VScrollBar verticalScrollBar = control.VerticalScrollBar;
            bool horizontalVisible = false;
            bool verticalVisible = false;
            float _zoomScale = (float)control.ZoomScale;
            // If the image is wider than the PictureBox, show the HScrollBar.
            horizontalScrollBar.Visible =
               (int)(contentSize.Width * _zoomScale) > clientSize.Width;

            // If the image is taller than the PictureBox, show the VScrollBar.
            verticalScrollBar.Visible =
               (int)(contentSize.Height * _zoomScale) > clientSize.Height + (horizontalScrollBar.Visible ? horizontalScrollBar.Height : 0);

            if (!horizontalScrollBar.Visible)
                horizontalScrollBar.Visible =
               (int)(contentSize.Width * _zoomScale) > clientSize.Width + (verticalScrollBar.Visible ? verticalScrollBar.Width : 0);

            #endregion

            // Set the Maximum, LargeChange and SmallChange properties.
            if (horizontalScrollBar.Visible)
            {  // If the offset does not make the Maximum less than zero, set its value.            
                horizontalScrollBar.Maximum =
                   contentSize.Width -
                   (int)(Math.Max(0, clientSize.Width - (verticalScrollBar.Visible ? verticalScrollBar.Width : 0)) / _zoomScale);
            }
            else
            {
                horizontalScrollBar.Maximum = 0;
            }
            horizontalScrollBar.LargeChange = (int)Math.Max(horizontalScrollBar.Maximum / 10, 1);
            horizontalScrollBar.SmallChange = (int)Math.Max(horizontalScrollBar.Maximum / 20, 1);

            if (verticalScrollBar.Visible)
            {  // If the offset does not make the Maximum less than zero, set its value.            
                verticalScrollBar.Maximum =
                   contentSize.Height -
                   (int)(Math.Max(0, clientSize.Height - (horizontalScrollBar.Visible ? horizontalScrollBar.Height : 0)) / _zoomScale);
            }
            else
            {
                verticalScrollBar.Maximum = 0;
            }
            verticalScrollBar.Minimum = -100;
            verticalScrollBar.LargeChange = (int)Math.Max(verticalScrollBar.Maximum / 10, 1);
            verticalScrollBar.SmallChange = (int)Math.Max(verticalScrollBar.Maximum / 20, 1);
        }

        public static void SetInterpolation(this Graphics g, InterpolationMode interpolation)
        {
            if (g.InterpolationMode != interpolation)
                g.InterpolationMode = interpolation;
        }

        public static void ZoomTransform(this Graphics g, float zoomScale)
        {
            if (zoomScale != 1.0)
                using (Matrix transform = g.Transform)
                {
                    transform.Scale(zoomScale, zoomScale, MatrixOrder.Append);

                    g.Transform = transform;
                }
        }

        public static void ZoomTransform(this Graphics g, Point offset)
        {
            if (!offset.IsEmpty)
                using (Matrix transform = g.Transform)
                {
                    transform.Translate(-offset.X, -offset.Y);
                    g.Transform = transform;
                }
        }

        public static void ZoomTransform(this Graphics g, float zoomScale, Point offset)
        {
            using (Matrix transform = g.Transform)
            {
                if (zoomScale != 1.0 && zoomScale != 0)
                    transform.Scale(zoomScale, zoomScale, MatrixOrder.Append);
                if (!offset.IsEmpty)
                    transform.Translate(-offset.X, -offset.Y);
                g.Transform = transform;
            }
        }
    }

    public class ZoomPictureBox : PictureBox, IZoomable
    {
        public ZoomPictureBox()
        {
            horizontalScrollBar = new HScrollBar();
            horizontalScrollBar.Dock = DockStyle.Bottom;


            verticalScrollBar = new VScrollBar();
            verticalScrollBar.Dock = DockStyle.Right;

            Controls.Add(horizontalScrollBar);
            Controls.Add(verticalScrollBar);
            this.SetScrollBarVisibilityAndMaxMin(Size.Empty, ClientSize);

            this.MouseEnter += new EventHandler((o, e) =>
            {
                this.Focus();
            });

            verticalScrollBar.Scroll += OnScrool;
            horizontalScrollBar.Scroll += OnScrool;
        }

        private void OnScrool(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        private readonly HScrollBar horizontalScrollBar;
        private readonly VScrollBar verticalScrollBar;
        private InterpolationMode interpolationMode;

        public double ZoomScale { get; private set; } = 1;
        public Point Offset => new Point(OffsetX, OffsetY);
        public int OffsetX => horizontalScrollBar.Visible ? horizontalScrollBar.Value : 0;
        public int OffsetY => verticalScrollBar.Visible ? verticalScrollBar.Value : 0;
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = this.InterpolationMode;
            ZoomableGraphics.ZoomTransform(pe.Graphics, (float)ZoomScale, Offset);
            base.OnPaint(pe);
            if (PaintNotScale != null)
            {
                ZoomableGraphics.ZoomTransform(pe.Graphics, 1 / (float)ZoomScale, Offset);
                OnPaintNotScale(pe);
            }
        }

        protected void OnPaintNotScale(PaintEventArgs pe)
        {
            PaintNotScale?.Invoke(this, pe);
        }

        public HScrollBar HorizontalScrollBar => horizontalScrollBar;
        public InterpolationMode InterpolationMode
        {
            get => interpolationMode;
            set
            {
                interpolationMode = value;
                Invalidate();
            }
        }
        public VScrollBar VerticalScrollBar => verticalScrollBar;
        public double ZoomDelta { get; set; } = 1.5;

        public event EventHandler OnZoomScaleChange;

        public void SetZoomScale(double zoomScale, Point fixPoint)
        {
            ZoomScale = (float)zoomScale;
            if (Image == null) return;
            this.SetScrollBarVisibilityAndMaxMin(Image.Size, new Size(20, 20));
            OnZoomScaleChange?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }



        public event EventHandler<PaintEventArgs> PaintNotScale;


        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.MouseWheelSetZoomScale(e, ZoomDelta);

        }

        public void UpZoom()
        {
            SetZoomScale(ZoomScale * ZoomDelta, Point.Empty);
        }

        public void LowZoom()
        {
            SetZoomScale(ZoomScale * (1 / ZoomDelta), Point.Empty);
        }

    }


    public interface IZoomable
    {
        HScrollBar HorizontalScrollBar { get; }
        InterpolationMode InterpolationMode { get; set; }
        VScrollBar VerticalScrollBar { get; }
        double ZoomDelta { get; set; }
        double ZoomScale { get; }
        event EventHandler OnZoomScaleChange;

        void SetZoomScale(double zoomScale, Point fixPoint);
    }
}

