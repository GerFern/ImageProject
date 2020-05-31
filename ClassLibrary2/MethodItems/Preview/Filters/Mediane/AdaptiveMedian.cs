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
    internal class AdaptiveMedian : ImageMethod
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            var image = (IMatrixImage)input.Clone();
            int length = Width * Height;
            int offset = (Width * Height) / 2;
            if (image is MatrixImage<byte> imgByte)
            {
                foreach (var layer in imgByte.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, a =>
                    {
                        byte min = a[0], max = a[0];
                        for (int i = 1; i < offset; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        for (int i = offset + 1; i < length; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        if (a[offset] > max || a[offset] < min)
                            return a.OrderBy(a => a).ElementAt(offset);
                        else return a[offset];
                    });
            }
            else if (image is MatrixImage<short> imgInt16)
            {
                foreach (var layer in imgInt16.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, a =>
                    {
                        short min = a[0], max = a[0];
                        for (int i = 1; i < offset; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        for (int i = offset + 1; i < length; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        if (a[offset] > max || a[offset] < min)
                            return a.OrderBy(a => a).ElementAt(offset);
                        else return a[offset];
                    });
            }
            else if (image is MatrixImage<int> imgInt32)
            {
                foreach (var layer in imgInt32.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, a =>
                    {
                        int min = a[0], max = a[0];
                        for (int i = 1; i < offset; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        for (int i = offset + 1; i < length; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        if (a[offset] > max || a[offset] < min)
                            return a.OrderBy(a => a).ElementAt(offset);
                        else return a[offset];
                    });
            }
            else if (image is MatrixImage<long> imgInt64)
            {
                foreach (var layer in imgInt64.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, a =>
                    {
                        long min = a[0], max = a[0];
                        for (int i = 1; i < offset; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        for (int i = offset + 1; i < length; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        if (a[offset] > max || a[offset] < min)
                            return a.OrderBy(a => a).ElementAt(offset);
                        else return a[offset];
                    });
            }
            else if (image is MatrixImage<float> imgSingle)
            {
                foreach (var layer in imgSingle.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, a =>
                    {
                        float min = a[0], max = a[0];
                        for (int i = 1; i < offset; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        for (int i = offset + 1; i < length; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        if (a[offset] > max || a[offset] < min)
                            return a.OrderBy(a => a).ElementAt(offset);
                        else return a[offset];
                    });
            }
            else if (image is MatrixImage<double> imgDouble)
            {
                foreach (var layer in imgDouble.SplitWithoutAlpha())
                    layer.SlidingWindow(Width, Height, a =>
                    {
                        double min = a[0], max = a[0];
                        for (int i = 1; i < offset; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        for (int i = offset + 1; i < length; i++)
                        {
                            if (a[i] < min) min = a[i];
                            else if (a[i] > max) max = a[i];
                        }
                        if (a[offset] > max || a[offset] < min)
                            return a.OrderBy(a => a).ElementAt(offset);
                        else return a[offset];
                    });
            }
            else throw new NotSupportedException();
            return image;
        }
    }
}