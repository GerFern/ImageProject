using ImageLib.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shared.WinFormsPlatform
{
    public partial class MatrixImageView : UserControl
    {
        private float zoomScaleX = 1;
        private float zoomScaleY = 1;
        private float offsetX = 0;
        private float offsetY = 0;
        private Point cursorOld;
        private bool dragMode = false;

        private int imageWidth;
        private int imageHeight;
        private InterpolationMode interpolationMode;

        public InterpolationMode InterpolationMode
        {
            get => interpolationMode;
            set
            {
                interpolationMode = value;
                Refresh();
            }
        }

        public float ZoomScaleX
        {
            get => zoomScaleX;
            set
            {
                zoomScaleX = value;
                if (zoomScaleX > 20f) zoomScaleX = 20f;
                else if (zoomScaleX < 0.05f) zoomScaleX = 0.05f;
                Refresh();
            }
        }

        public float ZoomScaleY
        {
            get => zoomScaleY;
            set
            {
                zoomScaleY = value;
                if (zoomScaleY > 20f) zoomScaleY = 20f;
                else if (zoomScaleY < 0.05f) zoomScaleY = 0.05f;
                Refresh();
            }
        }

        public float OffsetX
        {
            get => offsetX;
            set
            {
                offsetX = value;
                Refresh();
            }
        }

        public float OffsetY
        {
            get => offsetY;
            set
            {
                offsetY = value;
                Refresh();
            }
        }

        public void SetScale(float scaleX, float scaleY)
        {
            zoomScaleX = scaleX;
            zoomScaleY = scaleY;
            if (zoomScaleX > 20f) zoomScaleX = 20f;
            else if (zoomScaleX < 0.05f) zoomScaleX = 0.05f;
            if (zoomScaleY > 20f) zoomScaleY = 20f;
            else if (zoomScaleY < 0.05f) zoomScaleY = 0.05f;
        }

        public void SetOffset(float offsetX, float offsetY)
        {
            //offsetX = X;
            //offsetY = Y;

            if (offsetX < -imageWidth * zoomScaleX) offsetX = -imageWidth * zoomScaleX;
            else if (offsetX > imageWidth * zoomScaleX) offsetX = imageWidth * zoomScaleX;
            if (offsetY <= -imageHeight * zoomScaleY) offsetY = -imageHeight * zoomScaleY;
            else if (offsetY > imageHeight * zoomScaleY) offsetY = imageHeight * zoomScaleY;

            this.offsetX = offsetX;
            this.offsetY = offsetY;
        }

        public void ScrollBarUpdate()
        {
            bool h = imageWidth * zoomScaleX > ClientSize.Width;
            bool v = imageHeight * zoomScaleY > ClientSize.Height;
            if (h)
            {
                int max = imageWidth - (int)(Math.Max(0, ClientSize.Width - (v ? vScrollBar.Width : 0)) / zoomScaleX);
                hScrollBar.Maximum = max;
                hScrollBar.LargeChange = Math.Max(max / 10, 1);
                hScrollBar.SmallChange = Math.Max(max / 20, 1);
            }
            if (v)
            {
                int max = imageHeight - (int)(Math.Max(0, ClientSize.Height - (h ? hScrollBar.Height : 0)) / zoomScaleY);
                vScrollBar.Maximum = max;
                vScrollBar.LargeChange = Math.Max(max / 10, 1);
                vScrollBar.SmallChange = Math.Max(max / 20, 1);
            }

            vScrollBar.Top = 0;
            vScrollBar.Left = this.Width - vScrollBar.Width;

            hScrollBar.Top = this.Height - hScrollBar.Height;
            hScrollBar.Left = 0;

            if (v && h)
            {
                panel.Visible = true;
                hScrollBar.Width = Width - vScrollBar.Width;
                vScrollBar.Height = Height - hScrollBar.Height;
                panel.Location = new Point(Width - panel.Width, Height - panel.Height);
            }
            else
            {
                panel.Visible = false;
                hScrollBar.Width = Width;
                vScrollBar.Height = Height;
            }

            hScrollBar.Visible = h;
            vScrollBar.Visible = v;
        }

        private MainController controller;

        public MainController Controller
        {
            get => controller;
            set
            {
                var old = controller;
                if (controller != null)
                    controller.ImageHandlerChanged -= ImageHandlerChanged;
                controller = value;
                if (value != null)
                {
                    try
                    {
                        value.ImageHandlerChanged += ImageHandlerChanged;
                        BitmapHandler = (BitmapHandler)value.CurrentHandler;
                    }
                    catch
                    {
                        controller = old;
                        controller.ImageHandlerChanged += ImageHandlerChanged;
                        value.ImageHandlerChanged -= ImageHandlerChanged;
                        throw;
                    }
                }
                else BitmapHandler = null;
            }
        }

        private void ImageHandlerChanged(object sender, ImageLib.Image.ImageHandler e)
        {
            BitmapHandler = e as BitmapHandler;
            BitmapHandlerChanged?.Invoke(this, BitmapHandler);
        }

        public BitmapHandler BitmapHandler
        {
            get => bitmapHandler;
            set
            {
                if (bitmapHandler != null)
                    bitmapHandler.Updated -= Value_Updated;
                bitmapHandler = value;
                if (value != null)
                {
                    value.Updated += Value_Updated;
                    imageHeight = bitmapHandler.Bitmap.Height;
                    imageWidth = bitmapHandler.Bitmap.Width;
                }
                if (InvokeRequired)
                {
                    Invoke((Action)(() =>
                    {
                        ScrollBarUpdate();
                        Refresh();
                    }));
                }
                else
                {
                    ScrollBarUpdate();
                    Refresh();
                }
            }
        }

        private void Value_Updated(object sender, ImageLib.Image.UpdateImage e)
        {
            MainController.Platform.SynchronizeUI(() => Refresh());
        }

        private BitmapHandler bitmapHandler;

        public MatrixImageView()
        {
            BackgroundImage = CreateBackground();
            BackgroundImageLayout = ImageLayout.Tile;
            ResizeRedraw = false;
            DoubleBuffered = true;
            AutoScroll = false;
            InitializeComponent();
            hScrollBar.ValueChanged += (_, __) =>
            {
                offsetX = -hScrollBar.Value;
                Refresh();
            };
            vScrollBar.ValueChanged += (_, __) =>
            {
                offsetY = -vScrollBar.Value;
                Refresh();
            };
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            System.Diagnostics.Debug.WriteLine($"{se.Type} : {se.NewValue}");
            switch (se.ScrollOrientation)
            {
                case ScrollOrientation.HorizontalScroll:
                    offsetX = -se.NewValue;
                    HorizontalScroll.Value = (int)-offsetX;
                    break;

                case ScrollOrientation.VerticalScroll:
                    offsetY = -se.NewValue;
                    break;
            }
            Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            ScrollBarUpdate();
            base.OnSizeChanged(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta > 0)
            {
                zoomScaleX = (zoomScaleX) * 1.25f;
                zoomScaleY = (zoomScaleY) * 1.25f;
                //offsetX -= 1;
                //offsetX += (offsetX - e.X) / 1.25f;
                //System.Diagnostics.Debug.WriteLine($"{offsetX} : {e.X}");
                //offsetY += (offsetY - e.Y);
            }
            else if (e.Delta < 0)
            {
                zoomScaleX = zoomScaleX / 1.25f;
                zoomScaleY = zoomScaleY / 1.25f;
            }

            if (zoomScaleX > 20f) zoomScaleX = 20f;
            else if (zoomScaleX < 0.05f) zoomScaleX = 0.05f;
            if (zoomScaleY > 20f) zoomScaleY = 20f;
            else if (zoomScaleY < 0.05f) zoomScaleY = 0.05f;
            ScrollBarUpdate();
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (bitmapHandler != null)
            {
                try
                {
                    var g = e.Graphics;
                    g.InterpolationMode = interpolationMode;
                    g.ScaleTransform(zoomScaleX, zoomScaleY);
                    g.TranslateTransform(offsetX, offsetY);
                    g.DrawImage(bitmapHandler.Bitmap, Point.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                //dragMode = true;
                cursorOld = e.Location;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            dragMode = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dragMode)
            {
                var point = e.Location;
                SetOffset(offsetX - (cursorOld.X - point.X), offsetY - (cursorOld.Y - point.Y));
                //offsetX -= (cursorOld.X - point.X);// / zoomScaleX;
                //offsetY -= (cursorOld.Y - point.Y);// / zoomScaleY;
                cursorOld = point;
                //if (offsetX < -bitmapHandler.Bitmap.Width) offsetX = -bitmapHandler.Bitmap.Width;
                //else if (offsetX > bitmapHandler.Bitmap.Width) offsetX = bitmapHandler.Bitmap.Width;
                //if (offsetY <= -bitmapHandler.Bitmap.Height) offsetY = -bitmapHandler.Bitmap.Height;
                //else if (offsetY > bitmapHandler.Bitmap.Height) offsetY = bitmapHandler.Bitmap.Height;

                Refresh();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            this.offsetX = 0;
            this.offsetY = 0;
            this.zoomScaleX = 1;
            this.zoomScaleY = 1;
            Refresh();
        }

        /// <summary>
        /// Создать тайл фоновой плитки
        /// </summary>
        public static Bitmap CreateBackground(Color? first = null, Color? second = null)
        {
            Bitmap bitmap = new Bitmap(20, 20, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bitmap.Palette;
            Color[] ent = palette.Entries;
            ent[0] = first ?? Color.White;
            ent[1] = second ?? Color.LightGray;
            bitmap.Palette = palette;
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format8bppIndexed);
            IntPtr intPtr = bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int bytes = Math.Abs(stride) * bitmap.Height;
            byte[] vs = new byte[bytes]; // Bitmap storage
            for (int i = 0; i < 10; i++)
            {
                int v = i * stride;
                for (int j = 10; j < 20; j++)
                {
                    vs[v + j] = 1;
                }
            }
            for (int i = 10; i < 20; i++)
            {
                int v = i * stride;
                for (int j = 0; j < 10; j++)
                {
                    vs[v + j] = 1;
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(vs, 0, intPtr, bytes);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public event EventHandler<BitmapHandler> BitmapHandlerChanged;
    }
}