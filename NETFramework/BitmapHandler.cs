using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ImageLib.Image;

namespace NETFramework
{
    public class BitmapHandler : ImageHandler
    {
        public Bitmap Bitmap { get; }
        protected override void OnUpdate(UpdateImage updateImage)
        {
            Bitmap.FillBitmap(updateImage.Image);
            base.OnUpdate(updateImage);
        }
       
        public BitmapHandler(IMatrixImage matrixImage) 
            : base(matrixImage)
        {
            MatrixLayer<byte>[] layers = matrixImage.Split(false).Select(a => a.ToByteLayer(false)).ToArray();
            if (layers.Length == 1)
            {
                // GrayScale
                Bitmap = new Bitmap(matrixImage.Width, matrixImage.Height, PixelFormat.Format8bppIndexed);
                ColorPalette palette = Bitmap.Palette;
                Color[] ent = palette.Entries;
                for (int i = 0; i < 256; i++)
                {
                    ent[i] = Color.FromArgb(i, i, i); // Настроить для Bitmap только цвета с оттенками серого
                }
                Bitmap.Palette = palette;
                Bitmap.FillGrayBitmap(matrixImage.Split(false)[0]);
                matrixImage.Updated += (_, upd) => Bitmap.FillGrayBitmap(upd.Image.Split(false)[0]);
            }
            else if (layers.Length == 3)
            {
                // Bgr
                Bitmap = new Bitmap(matrixImage.Width, matrixImage.Height, PixelFormat.Format24bppRgb);
                Bitmap.FillBgrBitmap(matrixImage.Split(false));
                matrixImage.Updated += (_, upd) => Bitmap.FillBgrBitmap(upd.Image.Split(false));
            }
            else if (layers.Length == 4)
            {
                // Bgra
                Bitmap = new Bitmap(matrixImage.Width, matrixImage.Height, PixelFormat.Format32bppPArgb);
                Bitmap.FillBgraBitmap(matrixImage.Split(false));
                matrixImage.Updated += (_, upd) => Bitmap.FillBgraBitmap(upd.Image.Split(false));
            }
            else throw new NotSupportedException();
        }
    }
}
