using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;

namespace PlatformImpl.WinForms
{
    public static partial class BitmapUtils
    {
        public static BitmapHandler CreateBitmap(this IMatrixImage matrixImage)
        {
            return new BitmapHandler(matrixImage);
            //MatrixLayer<byte>[] layers = matrixImage.Split(false).Select(a => a.ToByteLayer(false)).ToArray();
            //Bitmap bitmap;
            //if (layers.Length == 1)
            //{
            //    // GrayScale
            //    bitmap = new Bitmap(matrixImage.Width, matrixImage.Height, PixelFormat.Format8bppIndexed);
            //    ColorPalette palette = bitmap.Palette;
            //    Color[] ent = palette.Entries;
            //    for (int i = 0; i < 256; i++)
            //    {
            //        ent[i] = Color.FromArgb(i, i, i); // Настроить для Bitmap только цвета с оттенками серого
            //    }
            //    bitmap.Palette = palette;
            //    bitmap.FillGrayBitmap(matrixImage.Split(false)[0]);
            //    return new BitmapHandler(bitmap, matrixImage, a => bitmap.FillGrayBitmap(a.Image.Split(false)[0]));
            //}
            //else if (layers.Length == 3)
            //{
            //    // RGB
            //    bitmap = new Bitmap(matrixImage.Width, matrixImage.Height, PixelFormat.Format24bppRgb);
            //    bitmap.FillBgrBitmap(matrixImage.Split(false));
            //    return new BitmapHandler(bitmap, matrixImage, a => bitmap.FillBgrBitmap(a.Image.Split(false)));
            //}
            //else if (layers.Length == 4)
            //{
            //    // RGBA
            //    bitmap = new Bitmap(matrixImage.Width, matrixImage.Height, PixelFormat.Format32bppPArgb);
            //    bitmap.FillBgraBitmap(matrixImage.Split(false));
            //    return new BitmapHandler(bitmap, matrixImage, a => bitmap.FillAbgrBitmap(a.Image.Split(false)));
            //}
            //throw new NotImplementedException();
        }

        public static MatrixImage<byte> ToMatrixImage(this Bitmap bitmap)
        {
            BitmapData bitmapData;
            IntPtr intPtr;
            int stride;
            int bytes;
            byte[] vs;
            byte[][] storages;
            MatrixLayer<byte>[] layers;
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                ImageLockMode.WriteOnly,
                                                PixelFormat.Format8bppIndexed);
                intPtr = bitmapData.Scan0;
                stride = bitmapData.Stride;
                bytes = Math.Abs(stride) * bitmap.Height;
                vs = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(intPtr, vs, 0, bytes);
                bitmap.UnlockBits(bitmapData);
                layers = new MatrixLayer<byte>[] { new MatrixLayer<byte>(bitmap.Width, bitmap.Height, vs, false) };
                return new MatrixImage<byte>(layers);
            }
            else if (bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            {
                bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                               ImageLockMode.WriteOnly,
                                               PixelFormat.Format24bppRgb);
                intPtr = bitmapData.Scan0;
                stride = bitmapData.Stride;
                bytes = Math.Abs(stride) * bitmap.Height;
                vs = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(intPtr, vs, 0, bytes);
                bitmap.UnlockBits(bitmapData);
                storages = new byte[3][];
                for (int i = 0; i < 3; i++)
                {
                    storages[i] = new byte[bitmap.Width * bitmap.Height];
                }
                int len = Math.Min(bitmap.Width * bitmap.Height * 3, vs.Length);
                for (int i = 0; i < len; i++)
                {
                    storages[i % 3][i / 3] = vs[i];
                }
                layers = new MatrixLayer<byte>[3];
                for (int i = 0; i < 3; i++)
                {
                    layers[i] = new MatrixLayer<byte>(bitmap.Width, bitmap.Height, storages[i],
                        bitmap.Width * bitmap.Height != storages[i].Length);
                }
                return new MatrixImage<byte>(layers);
            }
            else if (bitmap.PixelFormat == PixelFormat.Format32bppPArgb)
            {
                bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                               ImageLockMode.WriteOnly,
                                               PixelFormat.Format32bppPArgb);
                intPtr = bitmapData.Scan0;
                stride = bitmapData.Stride;
                bytes = Math.Abs(stride) * bitmap.Height;
                vs = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(intPtr, vs, 0, bytes);
                bitmap.UnlockBits(bitmapData);
                storages = new byte[4][];
                for (int i = 0; i < 4; i++)
                {
                    storages[i] = new byte[bitmap.Width * bitmap.Height];
                }
                int len = Math.Min(bitmap.Width * bitmap.Height * 4, vs.Length);

                for (int i = 0; i < len; i++)
                {
                    storages[i % 4][i / 4] = vs[i];
                }
                layers = new MatrixLayer<byte>[4];
                for (int i = 0; i < 4; i++)
                {
                    layers[i] = new MatrixLayer<byte>(bitmap.Width, bitmap.Height, storages[i], false);
                }
                return new MatrixImage<byte>(layers);
            }
            else if (bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                               ImageLockMode.WriteOnly,
                                               PixelFormat.Format32bppArgb);
                intPtr = bitmapData.Scan0;
                stride = bitmapData.Stride;
                bytes = Math.Abs(stride) * bitmap.Height;
                vs = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(intPtr, vs, 0, bytes);
                bitmap.UnlockBits(bitmapData);
                storages = new byte[4][];
                for (int i = 0; i < 4; i++)
                {
                    storages[i] = new byte[vs.Length / 4];
                }
                for (int i = 0; i < vs.Length; i++)
                {
                    storages[i % 4][i / 4] = vs[i];
                }
                layers = new MatrixLayer<byte>[4];
                for (int i = 0; i < 4; i++)
                {
                    layers[i] = new MatrixLayer<byte>(bitmap.Width, bitmap.Height, storages[i], false);
                }
                return new MatrixImage<byte>(layers);
            }
            throw new NotSupportedException();
        }

        public static void FillBitmap(this Bitmap bitmap, IMatrixImage image)
        {
            bitmap.FillBitmap(image.Split(false));
        }

        public static void FillBitmap(this Bitmap bitmap, IMatrixLayer[] layers)
        {
            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    bitmap.FillGrayBitmap(layers[0]);
                    break;

                case PixelFormat.Format24bppRgb:
                    bitmap.FillBgrBitmap(layers);
                    break;

                case PixelFormat.Format32bppPArgb:
                    bitmap.FillBgraBitmap(layers);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public static void FillGrayBitmap(this Bitmap bitmap, IMatrixLayer layer)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format8bppIndexed);
            IntPtr intPtr = bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int bytes = Math.Abs(stride) * bitmap.Height;

            byte[] vs;// = new byte[bytes]; // Bitmap storage
            MatrixLayer<byte> byteLayer = layer.ToByteLayer(false);
            vs = byteLayer.GetStorage(false);
            if (bitmap.Width == stride) System.Runtime.InteropServices.Marshal.Copy(vs, 0, intPtr, vs.Length);
            else
            {
                int offsetArr = 0;
                int offsetPtr = 0;
                for (int i = 0; i < bitmap.Height; i++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(vs, offsetArr, (intPtr + offsetPtr), layer.Width);
                    offsetArr += layer.Width;
                    offsetPtr += stride;
                }
            }
            bitmap.UnlockBits(bitmapData);
        }

        public static void FillBgrBitmap(this Bitmap bitmap, IMatrixLayer[] layers)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format24bppRgb);
            IntPtr intPtr = bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int bytes = Math.Abs(stride) * bitmap.Height;

            byte[] vs = new byte[bytes]; // Bitmap storage
            MatrixLayer<byte>[] byteLayers = layers.Select(a => a.ToByteLayer(false)).ToArray();
            IEnumerator<byte>[] byteEnums = byteLayers.Select(a => a.GetEnumerator()).ToArray();

            for (int i = 0; i < vs.Length; i++)
            {
                var en = byteEnums[i % 3];
                en.MoveNext();
                vs[i] = en.Current;
            }

            System.Runtime.InteropServices.Marshal.Copy(vs, 0, intPtr, bytes);
            bitmap.UnlockBits(bitmapData);
        }

        public static void FillBgraBitmap(this Bitmap bitmap, IMatrixLayer[] layers)
        {
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                                                    ImageLockMode.WriteOnly,
                                                    PixelFormat.Format32bppPArgb);
            IntPtr intPtr = bitmapData.Scan0;
            int stride = bitmapData.Stride;
            int bytes = Math.Abs(stride) * bitmap.Height;

            byte[] vs = new byte[bytes]; // Bitmap storage
            MatrixLayer<byte>[] byteLayers = layers.Select(a => a.ToByteLayer(false)).ToArray();
            IEnumerator<byte>[] byteEnums = byteLayers.Select(a => a.GetEnumerator()).ToArray();

            for (int i = 0; i < vs.Length; i++)
            {
                var en = byteEnums[i % 4];
                en.MoveNext();
                vs[i] = en.Current;
            }

            System.Runtime.InteropServices.Marshal.Copy(vs, 0, intPtr, bytes);
            bitmap.UnlockBits(bitmapData);
        }
    }
}