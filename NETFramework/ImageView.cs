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

namespace NETFramework
{
    public partial class ImageView : UserControl
    {
        public BitmapHandler BitmapHandler
        {
            get => bitmapHandler;
            set
            {
                if (bitmapHandler != null)
                    bitmapHandler.Updated -= Value_Updated;
                bitmapHandler = value;
                if (value != null)
                    value.Updated += Value_Updated;
                this.Invalidate(false);
            }
        }

        private void Value_Updated(object sender, ImageLib.Image.UpdateImage e)
        {
            this.Invalidate(false);
        }

        private BitmapHandler bitmapHandler;

        public ImageView()
        {
            BackgroundImage = CreateBackground(null, null);
            BackgroundImageLayout = ImageLayout.Tile;
            DoubleBuffered = true;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (bitmapHandler != null)
            {
                e.Graphics.DrawImage(bitmapHandler.Bitmap, Point.Empty);
            }
        }

        public Bitmap CreateBackground(Color? first, Color? second)
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