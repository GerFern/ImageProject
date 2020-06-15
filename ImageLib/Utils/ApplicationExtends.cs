using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ImageLib.Image;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ImageLib.Utils
{
    public static class ApplicationExtends
    {
        public static Window GetMainWindow(this Application application)
        {
            if (application.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }

        //public static async Task<string[]> ShowDialog(this FileDialog control, Window? owner = null)
        //{
        //    if (owner == null) owner = Application.Current.GetMainWindow();
        //    var result = await
        //}
        public static MatrixImage<byte> CreateMatrixImage(this SKBitmap bitmap)
        {
            MatrixImage<byte> image = null;
            switch (bitmap.ColorType)
            {
                case SKColorType.Unknown:
                    break;

                case SKColorType.Alpha8:
                    break;

                case SKColorType.Rgb565:
                    break;

                case SKColorType.Argb4444:
                    break;

                case SKColorType.Rgba8888:
                    break;

                case SKColorType.Rgb888x:
                    break;

                case SKColorType.Bgra8888:
                    image = new MatrixImage<byte>(bitmap.Width, bitmap.Height, 4);
                    var vs = bitmap.Bytes;
                    var len = vs.Length;
                    var storages = image.Split(false).Select(a => a.GetStorage(false)).ToArray();
                    for (int i = 0; i < len; i++)
                    {
                        storages[i % 4][i / 4] = vs[i];
                    }
                    break;

                case SKColorType.Rgba1010102:
                    break;

                case SKColorType.Rgb101010x:
                    break;

                case SKColorType.Gray8:
                    break;

                case SKColorType.RgbaF16:
                    break;

                default:
                    break;
            }
            return image;
        }
    }
}