/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ImageLib.Utils.ImageUtils;
using OpenCVSupport;
using System.Linq;
using System.ComponentModel;
using System.Linq.Expressions;

namespace NIRS.MethodItems.Preview.Filters.Mediane
{
    [Serializable]
    internal class Median : ImageMethod
    {
        [DisplayName("Использовать OpenCV")]
        public bool UseOpenCV { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public static TElement MedianeImpl<TElement>(TElement[] elements)
            where TElement : unmanaged, IComparable<TElement>
        {
            return elements.OrderBy(a => a).Skip(elements.Length / 2).First();
        }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            if (UseOpenCV)
            {
                OpenCvSharp.Mat mat = input.GetCVMat();
                input.SetCVMat(mat.MedianBlur(Width));
                mat.Dispose();
                return input;
            }
            else
            {
                input.SlidingWindow(new MedianeOperation(Width, Height));
                //int offset = (Width * Height) / 2;
                //if (image is MatrixImage<byte> imgByte)
                //{
                //    foreach (var layer in imgByte.SplitWithoutAlpha())
                //        layer.SlidingWindow(Width, Height, a => a.OrderBy(a => a).ElementAt(offset));
                //}
                //else if (image is MatrixImage<short> imgInt16)
                //{
                //    foreach (var layer in imgInt16.SplitWithoutAlpha())
                //        layer.SlidingWindow(Width, Height, a => a.OrderBy(a => a).ElementAt(offset));
                //}
                //else if (image is MatrixImage<int> imgInt32)
                //{
                //    foreach (var layer in imgInt32.SplitWithoutAlpha())
                //        layer.SlidingWindow(Width, Height, a => a.OrderBy(a => a).ElementAt(offset));
                //}
                //else if (image is MatrixImage<long> imgInt64)
                //{
                //    foreach (var layer in imgInt64.SplitWithoutAlpha())
                //        layer.SlidingWindow(Width, Height, a => a.OrderBy(a => a).ElementAt(offset));
                //}
                //else if (image is MatrixImage<float> imgSingle)
                //{
                //    foreach (var layer in imgSingle.SplitWithoutAlpha())
                //        layer.SlidingWindow(Width, Height, a => a.OrderBy(a => a).ElementAt(offset));
                //}
                //else if (image is MatrixImage<double> imgDouble)
                //{
                //    foreach (var layer in imgDouble.SplitWithoutAlpha())
                //        layer.SlidingWindow(Width, Height, a => a.OrderBy(a => a).ElementAt(offset));
                //}
                //else throw new NotSupportedException();
                return input;
            }
        }
    }

    public class MedianeOperation : OperationSequence
    {
        public MedianeOperation(int width, int height) : base(width, height)
        {
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            var array = Expression.Parameter(typeElement.MakeArrayType(), "array");
            return Expression.Block(new[] { array }, new Expression[]
            {
                ExpCalculateTmpArray(array),
                ExpressionExtends.SortArray(array, 0, CoreWidth * CoreHeight - 1),
                Expression.ArrayIndex(array, Expression.Constant(CoreWidth * CoreHeight - 1))
            });
        }
    }
}