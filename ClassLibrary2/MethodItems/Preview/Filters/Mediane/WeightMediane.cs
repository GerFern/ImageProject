/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NIRS.MethodItems.Preview.Filters.Mediane
{
    [Serializable]
    public class WeightedMedian : ImageMethod
    {
        public override IMatrixImage Invoke(IMatrixImage input)
        {
            var image = (IMatrixImage)input.Clone();

            int offset = (3 * 3) / 2;
            if (image is MatrixImage<byte> imgByte)
            {
                foreach (var layer in imgByte.SplitWithoutAlpha())
                    layer.SlidingWindow(3, 3, a =>
                    {
                        var t = a[0] * 3;
                        if (t > byte.MaxValue) t = byte.MaxValue;
                        a[0] = (byte)t;
                        return a.OrderBy(a => a).ElementAt(offset);
                    });
            }
            else if (image is MatrixImage<short> imgInt16)
            {
                foreach (var layer in imgInt16.SplitWithoutAlpha())
                    layer.SlidingWindow(3, 3, a =>
                    {
                        var t = a[0] * 3;
                        if (t > short.MaxValue) t = short.MaxValue;
                        a[0] = (short)t;
                        return a.OrderBy(a => a).ElementAt(offset);
                    });
            }
            else if (image is MatrixImage<int> imgInt32)
            {
                foreach (var layer in imgInt32.SplitWithoutAlpha())
                    layer.SlidingWindow(3, 3, a =>
                    {
                        a[0] *= 3;
                        return a.OrderBy(a => a).ElementAt(offset);
                    });
            }
            else if (image is MatrixImage<long> imgInt64)
            {
                foreach (var layer in imgInt64.SplitWithoutAlpha())
                    layer.SlidingWindow(3, 3, a =>
                    {
                        a[0] *= 3;
                        return a.OrderBy(a => a).ElementAt(offset);
                    });
            }
            else if (image is MatrixImage<float> imgSingle)
            {
                foreach (var layer in imgSingle.SplitWithoutAlpha())
                    layer.SlidingWindow(3, 3, a =>
                    {
                        a[0] *= 3;
                        return a.OrderBy(a => a).ElementAt(offset);
                    });
            }
            else if (image is MatrixImage<double> imgDouble)
            {
                foreach (var layer in imgDouble.SplitWithoutAlpha())
                    layer.SlidingWindow(3, 3, a =>
                    {
                        a[0] *= 3;
                        return a.OrderBy(a => a).ElementAt(offset);
                    });
            }
            else throw new NotSupportedException();
            return image;
        }
    }

    //public class WeightedMedianOperator
    //{
    //    public double[,] vs
    //}
}