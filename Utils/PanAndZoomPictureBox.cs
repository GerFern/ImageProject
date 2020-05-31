using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Utils
{
    public static class ZoomableGraphics
    {

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
                    transform.Translate(offset.X, offset.Y);
                    g.Transform = transform;
                }
        }

        public static void ZoomTransform(this Graphics g, float zoomScale, Point offset)
        {
            using (Matrix transform = g.Transform)
            {
                if (zoomScale != 1.0)
                    transform.Scale(zoomScale, zoomScale, MatrixOrder.Append);
                if (!offset.IsEmpty)
                    transform.Translate(offset.X, offset.Y);
                g.Transform = transform;
            }
        }
    }



    /// <summary>
    /// A picture box with pan and zoom functionality
    /// </summary>
    public class PanAndZoomPictureBox : Control, IZoomable
    {
        private Size contentSize;
        private bool _panableAndZoomable;
        /// <summary>
        /// The zoom scale of the image to be displayed
        /// </summary>
        private double _zoomScale;

        private Point _mouseDownPosition;
        private MouseButtons _mouseDownButton;
        private Point _bufferPoint;
        private HScrollBar horizontalScrollBar;
        private VScrollBar verticalScrollBar;
        private InterpolationMode _interpolationMode = InterpolationMode.NearestNeighbor;
        private static readonly Cursor _defaultCursor = Cursors.Cross;

        /// <summary>
        /// The available zoom levels for the displayed image 
        /// </summary>
        public static double[] ZoomLevels = new double[] { 0.125, 0.25, 0.5, 1.0, 2.0, 4.0, 8.0 };
        private double zoomDelta;

        int OffsetX => horizontalScrollBar.Visible ? horizontalScrollBar.Value : 0;
        int OffsetY => verticalScrollBar.Visible ? verticalScrollBar.Value : 0;

        public Point CursorPoint => TranslatePoint(PointToClient(Cursor.Position));
        public PointF CursorPointF => TranslatePointF(PointToClient(Cursor.Position));

        //public PointF CursorPointF => new PointF(OffsetXF, OffsetYF);

        public double ZoomDelta { get => zoomDelta; set => zoomDelta = value; }
        public virtual Size ContentSize { get => contentSize; set => contentSize = value; }
        /// <summary>
        /// Create a picture box with pan and zoom functionality
        /// </summary>
        public PanAndZoomPictureBox()
        {
            InitializeComponent();
            _zoomScale = 1.0;

            SetScrollBarVisibilityAndMaxMin();
            //Enable double buffering
            ResizeRedraw = false;
            DoubleBuffered = true;
            //this.BorderStyle = BorderStyle.Fixed3D;
            PanableAndZoomable = true;
        }

        /// <summary>
        /// Get or Set the property to enable(disable) Pan and Zoom
        /// </summary>
        protected bool PanableAndZoomable
        {
            get
            {
                return _panableAndZoomable;
            }
            set
            {
                if (_panableAndZoomable != value)
                {
                    _panableAndZoomable = value;
                    if (_panableAndZoomable)
                    {
                        MouseEnter += OnMouseEnter;
                        MouseWheel += OnMouseWheel;
                        MouseMove += OnMouseMove;
                        Resize += OnResize;
                        MouseDown += OnMouseDown;
                        MouseUp += OnMouseUp;
                    }
                    else
                    {
                        MouseEnter -= OnMouseEnter;
                        MouseWheel -= OnMouseWheel;
                        MouseMove -= OnMouseMove;
                        Resize -= OnResize;
                        MouseDown -= OnMouseDown;
                        MouseUp -= OnMouseUp;
                    }
                }
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _mouseDownPosition = e.Location;
            _mouseDownButton = e.Button;

            _bufferPoint = Point.Empty;
            if (e.Button == MouseButtons.Middle)
                Cursor = Cursors.Hand;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Cursor = _defaultCursor;
            _mouseDownButton = MouseButtons.None;
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {  //set this as the active control 
            //Form f = TopLevelControl as Form;
            //if (f != null) f.ActiveControl = this;
        }

        private void OnResize(object sender, EventArgs e)
        {
            Size viewSize = GetViewSize();
            if (viewSize.Width > 0 && viewSize.Height > 0)
            {
                SetScrollBarVisibilityAndMaxMin();
                Invalidate();
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {  //handle the mouse whell scroll (for zooming)
            double scale = 1.0;
            if (e.Delta > 0)
            {
                scale = 2.0;
            }
            else if (e.Delta < 0)
            {
                scale = 0.5;
            }
            else
                return;

            SetZoomScale(ZoomScale * scale, e.Location);
        }

        /// <summary>
        /// The event to fire when the zoom scale is changed
        /// </summary>
        public event EventHandler OnZoomScaleChange;

        #region Handling ScrollBars

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        /// <summary>
        /// Get or Set the interpolation mode for zooming operation
        /// </summary>
        [Bindable(false)]
        [Category("Design")]
        [DefaultValue(InterpolationMode.NearestNeighbor)]
        public InterpolationMode InterpolationMode
        {
            get
            {
                return _interpolationMode;
            }
            set
            {
                _interpolationMode = value;
            }
        }


        public Rectangle VisibleRectangle => TranslateRectangle(ClientRectangle);

        public Point TranslatePointReverse(Point point)
        {
            return new Point((int)((point.X - OffsetX) * _zoomScale), (int)((point.Y - OffsetY) * _zoomScale));
        }

        public PointF TranslatePointFReverse(PointF point)
        {
            return new PointF((float)((point.X - OffsetX) * _zoomScale) , (float)((point.Y - OffsetY )* _zoomScale) );
        }

        public Point TranslatePoint(Point clientPoint)
        {
            return new Point(OffsetX + (int)(clientPoint.X / _zoomScale), OffsetY + (int)(clientPoint.Y / _zoomScale));
        }

        public PointF TranslatePointF(Point clientPoint)
        {
            return new PointF(OffsetX + (float)(clientPoint.X / _zoomScale), OffsetY + (float)(clientPoint.Y / _zoomScale));
        }

        public PointF TranslatePointF(PointF clientPoint)
        {
            return new PointF(OffsetX + (float)(clientPoint.X / _zoomScale), OffsetY + (float)(clientPoint.Y / _zoomScale));
        }

        public Rectangle TranslateRectangle(Rectangle rectangle)
        {
            Point first = rectangle.Location;
            Point second = rectangle.Location;
            second.Offset(rectangle.Width, rectangle.Height);
            first = TranslatePoint(first);
            second = TranslatePoint(second);
            second.Offset(-first.X, -first.Y);
            return new Rectangle(first, new Size(second));
        }

        public Rectangle TranslateRectangleReverse(Rectangle rectangle)
        {
            Point first = rectangle.Location;
            Point second = rectangle.Location;
            second.Offset(rectangle.Width, rectangle.Height);
            first = TranslatePointReverse(first);
            second = TranslatePointReverse(second);
            second.Offset(-first.X, -first.Y);
            return new Rectangle(first, new Size(second));
        }

        /// <summary>
        /// Paint the image
        /// </summary>
        /// <param name="pe">The paint event</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (IsDisposed) return;
            if (
              
               (_zoomScale != 1.0 ||
               (horizontalScrollBar.Visible && horizontalScrollBar.Value != 0) ||
               (verticalScrollBar.Visible && verticalScrollBar.Value != 0)))
            {
                if (pe.Graphics.InterpolationMode != _interpolationMode)
                    pe.Graphics.InterpolationMode = _interpolationMode;

                Point offset = new Point(
                    horizontalScrollBar.Visible ? -horizontalScrollBar.Value : 0,
                    verticalScrollBar.Visible ? -verticalScrollBar.Value : 0
                    );

                PaintBeforeZoom?.Invoke(this, pe);
                pe.Graphics.ZoomTransform((float)_zoomScale, offset);
                base.OnPaint(new PaintEventArgs(pe.Graphics, TranslateRectangle(pe.ClipRectangle)));
                if (PaintAfterZoom != null)
                {
                    pe.Graphics.ResetTransform();
                    //pe.Graphics.ZoomTransform(1/(float)_zoomScale, new Point(-offset.X, -offset.Y));
                    PaintAfterZoom.Invoke(this, pe);
                }
            }
            else
            {
                PaintBeforeZoom?.Invoke(this, pe);
                base.OnPaint(pe);
                PaintAfterZoom?.Invoke(this, pe);
            }
        }

        public event PaintEventHandler PaintBeforeZoom;
        public event PaintEventHandler PaintAfterZoom;

        private void SetScrollBarVisibilityAndMaxMin()
        {
            #region determine if the scroll bar should be visible or not
            bool horizontalVisible = false;
            bool verticalVisible = false;


            // If the image is wider than the PictureBox, show the HScrollBar.
            horizontalScrollBar.Visible =
               (int)(contentSize.Width * _zoomScale) > ClientSize.Width;

            // If the image is taller than the PictureBox, show the VScrollBar.
            verticalScrollBar.Visible =
               (int)(contentSize.Height * _zoomScale) > ClientSize.Height - (horizontalScrollBar.Visible ? horizontalScrollBar.Height : 0);

            if (!horizontalScrollBar.Visible)
                horizontalScrollBar.Visible =
               (int)(contentSize.Width * _zoomScale) > ClientSize.Width - (verticalScrollBar.Visible ? verticalScrollBar.Width : 0);

            #endregion

            // Set the Maximum, LargeChange and SmallChange properties.
            if (horizontalScrollBar.Visible)
            {  // If the offset does not make the Maximum less than zero, set its value.            
                horizontalScrollBar.Maximum =
                   contentSize.Width -
                   (int)(Math.Max(0, ClientSize.Width - (verticalScrollBar.Visible ? verticalScrollBar.Width : 0)) / _zoomScale);
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
                   (int)(Math.Max(0, ClientSize.Height - (horizontalScrollBar.Visible ? horizontalScrollBar.Height : 0)) / _zoomScale);
            }
            else
            {
                verticalScrollBar.Maximum = 0;
            }
            verticalScrollBar.LargeChange = (int)Math.Max(verticalScrollBar.Maximum / 10, 1);
            verticalScrollBar.SmallChange = (int)Math.Max(verticalScrollBar.Maximum / 20, 1);
        }
        #endregion

        /// <summary>
        /// Get the horizontal scroll bar from this control
        /// </summary>
        [Browsable(false)]
        public HScrollBar HorizontalScrollBar
        {
            get
            {
                return horizontalScrollBar;
            }
        }

        /// <summary>
        /// Get the vertical scroll bar of this control
        /// </summary>
        [Browsable(false)]
        public VScrollBar VerticalScrollBar
        {
            get
            {
                return verticalScrollBar;
            }
        }

        /// <summary>
        /// Used for tracking the mouse position on the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDownButton == MouseButtons.Middle && (horizontalScrollBar.Visible || verticalScrollBar.Visible))
            {
                int horizontalShift = (int)((e.X - _mouseDownPosition.X) / _zoomScale);
                int verticalShift = (int)((e.Y - _mouseDownPosition.Y) / _zoomScale);

                if (horizontalShift == 0 && verticalShift == 0) return;

                //if (horizontalScrollBar.Visible)
                horizontalScrollBar.Value =
                      Math.Max(Math.Min(horizontalScrollBar.Value - horizontalShift, horizontalScrollBar.Maximum), horizontalScrollBar.Minimum);
                //if (verticalScrollBar.Visible)
                verticalScrollBar.Value =
                      Math.Max(Math.Min(verticalScrollBar.Value - verticalShift, verticalScrollBar.Maximum), verticalScrollBar.Minimum);

                if (horizontalShift != 0) _mouseDownPosition.X = e.Location.X;
                if (verticalShift != 0) _mouseDownPosition.Y = e.Location.Y;

                Invalidate();
            }
        }

        /// <summary>
        /// Get the size of the view area
        /// </summary>
        /// <returns>The size of the view area</returns>
        protected internal Size GetViewSize()
        {
            return new Size(
               ClientSize.Width - (verticalScrollBar.Visible ? verticalScrollBar.Width : 0),
               ClientSize.Height - (horizontalScrollBar.Visible ? horizontalScrollBar.Height : 0));
        }

        /// <summary>
        /// Get the size of the image
        /// </summary>
        /// <returns>The size of the image</returns>
        protected internal Size GetImageSize()
        {

            return new Size(
               (int)Math.Round(contentSize.Width * _zoomScale),
               (int)Math.Round(contentSize.Height * _zoomScale));
        }

        /// <summary>
        /// Get the minimum of the two sizes
        /// </summary>
        /// <param name="s1">The first size</param>
        /// <param name="s2">The second size</param>
        /// <returns>The minimum of the two sizes</returns>
        protected internal static Size Min(Size s1, Size s2)
        {
            return new Size(
               s1.Width < s2.Width ? s1.Width : s2.Width,
               s1.Height < s2.Height ? s1.Height : s2.Height);
        }

        /// <summary>
        /// Draw a reversible rectangle on the control.
        /// </summary>
        /// <param name="rect">The rectangle using the control's coordinate system</param>
        public void DrawReversibleRectangle(Rectangle rect)
        {
            rect.Location = PointToScreen(rect.Location);
            ControlPaint.DrawReversibleFrame(
               rect,
               Color.White,
               FrameStyle.Dashed);
        }

        private Rectangle GetRectanglePreserveAspect(Point p1, Point p2)
        {
            Rectangle rect = GetRectangle(p1, p2);
            Size size = Min(GetViewSize(), GetImageSize());

            if ((double)rect.Width / rect.Height > (double)size.Width / size.Height)
                rect.Width = (int)((double)size.Width / size.Height * rect.Height);
            else if ((double)rect.Width / rect.Height < (double)size.Width / size.Height)
                rect.Height = (int)((double)rect.Width / size.Width * size.Height);

            if (rect.Y != p2.Y)
                rect.Y = p2.Y - rect.Height;

            if (rect.X != p2.X)
                rect.X = p2.X - rect.Width;
            return rect;
        }


        /// <summary>
        /// Get the rectangle defined by the two points on the control
        /// </summary>
        /// <param name="p1">The first point on the control</param>
        /// <param name="p2">The second point on the control</param>
        /// <returns>the rectangle defined by the two points</returns>
        public Rectangle GetRectangle(Point p1, Point p2)
        {
            int top = Math.Min(p1.Y, p2.Y);
            int bottom = Math.Max(p1.Y, p2.Y);
            int left = Math.Min(p1.X, p2.X);
            int right = Math.Max(p1.X, p2.X);

            Size size = Min(GetViewSize(), GetImageSize());

            Rectangle rect = new Rectangle(left, top, right - left, bottom - top);
            rect.Intersect(new Rectangle(Point.Empty, size));
            return rect;
        }

        /// <summary>
        /// Get or Set the zoom scale
        /// </summary>
        [Browsable(false)]
        public double ZoomScale
        {
            get
            {
                return _zoomScale;
            }
        }

        /// <summary>
        /// Set the new zoom scale for the displayed image
        /// </summary>
        /// <param name="zoomScale">The new zoom scale</param>
        /// <param name="fixPoint">The point to be fixed, in display coordinate</param>
        public void SetZoomScale(double zoomScale, Point fixPoint)
        {
            if (_zoomScale != zoomScale && zoomScale>0.005) //the scale has been changed
               //&& //and, the scale is not too small
               //!(zoomScale < _zoomScale &&
               //   (contentSize.Width * zoomScale < 2.0
               //   || contentSize.Height * zoomScale < 2.0))
               //&& //and, the scale is not too big
               //!(zoomScale > _zoomScale &&
               //   (GetViewSize().Width < zoomScale * 2
               //   || GetViewSize().Height < zoomScale * 2)))
            {
                //constrain the coordinate to be within valide range
                fixPoint.X = Math.Min(fixPoint.X, (int)(contentSize.Width * _zoomScale));
                fixPoint.Y = Math.Min(fixPoint.Y, (int)(contentSize.Height * _zoomScale));

                int shiftX = (int)(fixPoint.X * (zoomScale - _zoomScale) / zoomScale / _zoomScale);
                int shiftY = (int)(fixPoint.Y * (zoomScale - _zoomScale) / zoomScale / _zoomScale);

                _zoomScale = zoomScale;

                int h = (int)(horizontalScrollBar.Value + shiftX);
                int v = (int)(verticalScrollBar.Value + shiftY);
                SetScrollBarVisibilityAndMaxMin();
                horizontalScrollBar.Value = Math.Min(Math.Max(horizontalScrollBar.Minimum, h), horizontalScrollBar.Maximum); ;
                verticalScrollBar.Value = Math.Min(Math.Max(verticalScrollBar.Minimum, v), verticalScrollBar.Maximum);

                Invalidate();

                if (OnZoomScaleChange != null)
                    OnZoomScaleChange(this, new EventArgs());

            }
        }

        private void InitializeComponent()
        {
            this.horizontalScrollBar = new System.Windows.Forms.HScrollBar();
            this.verticalScrollBar = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // horizontalScrollBar
            // 
            this.horizontalScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.horizontalScrollBar.Location = new System.Drawing.Point(0, 0);
            this.horizontalScrollBar.Name = "horizontalScrollBar";
            this.horizontalScrollBar.Size = new System.Drawing.Size(80, 17);
            this.horizontalScrollBar.TabIndex = 2;
            this.horizontalScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
            this.Controls.Add(horizontalScrollBar);
            // 
            // verticalScrollBar
            // 
            this.verticalScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.verticalScrollBar.Location = new System.Drawing.Point(0, 0);
            this.verticalScrollBar.Name = "verticalScrollBar";
            this.verticalScrollBar.Size = new System.Drawing.Size(17, 80);
            this.verticalScrollBar.TabIndex = 1;
            this.verticalScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
            this.Controls.Add(verticalScrollBar);
            // 
            // PanAndZoomPictureBox
            // 

            this.ResumeLayout(false);

        }
    }


    public interface IZoomable
    {
        HScrollBar HorizontalScrollBar { get; }
        InterpolationMode InterpolationMode { get; set; }
        VScrollBar VerticalScrollBar { get; }
        double ZoomDelta { get; set; }

        event EventHandler OnZoomScaleChange;

        void SetZoomScale(double zoomScale, Point fixPoint);
    }
}

