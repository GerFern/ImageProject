/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;

using System;

namespace NIRS.MethodItems.Preview.Contours
{
    [Serializable]
    internal class Laplas : ImageMethod
    {
        private static byte Func(byte[] a)
        {
            int t = -a[0] - 2 * a[1] - a[2] - 2 * a[3] + 12 * a[4] - 2 * a[5] - a[6] - 2 * a[7] - a[8];
            if (t < 0) return 0;
            else if (t > 255) return 255;
            else return (byte)t;
        }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            MethodImpl.Preview.Contours.Implementations.Laplas(input);

            //var image = input.ToByteImage(true);
            //foreach (var layer in image.SplitWithoutAlpha())
            //    layer.SlidingWindow(3, 3, Func);
            return input;
        }
    }
}