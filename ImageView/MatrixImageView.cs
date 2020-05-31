/// Автор: Лялин Максим ИС-116
/// @2020

using Shared.WinFormsPlatform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageView
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

        private float ZoomScaleX
        {
            get => zoomScaleX;
            set
            {
                zoomScaleX = value;
                if (zoomScaleX > 10f) zoomScaleX = 10f;
                else if (zoomScaleX < 0.05f) zoomScaleX = 0.05f;
                Invalidate();
            }
        }

        private float ZoomScaleY
        {
            get => zoomScaleY;
            set
            {
                zoomScaleY = value;
                if (zoomScaleY > 10f) zoomScaleY = 10f;
                else if (zoomScaleY < 0.05f) zoomScaleY = 0.05f;
                Invalidate();
            }
        }

        public void SetScale(float scaleX, float scaleY)
        {
            zoomScaleX = scaleX;
            zoomScaleY = scaleY;
            if (zoomScaleX > 10f) zoomScaleX = 10f;
            else if (zoomScaleX < 0.05f) zoomScaleX = 0.05f;
            if (zoomScaleY > 10f) zoomScaleY = 10f;
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
                this.Invalidate(false);
            }
        }

        private void Value_Updated(object sender, ImageLib.Image.UpdateImage e)
        {
            this.Invalidate(false);
        }

        private BitmapHandler bitmapHandler;

        public MatrixImageView()
        {
            BackgroundImage = CreateBackground();
            BackgroundImageLayout = ImageLayout.Tile;
            DoubleBuffered = true;
            InitializeComponent();
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

            if (zoomScaleX > 10f) zoomScaleX = 10f;
            else if (zoomScaleX < 0.05f) zoomScaleX = 0.05f;
            if (zoomScaleY > 10f) zoomScaleY = 10f;
            else if (zoomScaleY < 0.05f) zoomScaleY = 0.05f;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bitmapHandler != null)
            {
                var g = e.Graphics;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.TranslateTransform(offsetX, offsetY);
                g.ScaleTransform(zoomScaleX, zoomScaleY);
                g.DrawImage(bitmapHandler.Bitmap, Point.Empty);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                dragMode = true;
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

                Invalidate();
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            this.offsetX = 0;
            this.offsetY = 0;
            this.zoomScaleX = 1;
            this.zoomScaleY = 1;
            Invalidate();
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
    }
}