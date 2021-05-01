/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib.Image;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenCVSupport
{
    public static class ImageExtensions
    {
        public const string tagCVMat = "CV_Mat";

        public static Mat GetCVMat(this IMatrixImage image, ColorConversionCodes? conversion = null, bool sync = true)
        {
            if (conversion != null)
            {
                var method = Info.OfMethod("OpenCVSupport", "OpenCVSupport.ImageExtensions", "GetCVMat", "MatrixImage`1, ColorConversionCodes");
                method = method.MakeGenericMethod(image.ElementType);
                //var method =
                //    typeof(ImageExtensions).GetMethod(nameof(GetCVMat), 1, new Type[] { typeof(MatrixImage<>) })
                //    .MakeGenericMethod(image.ElementType);
                return (Mat)method.Invoke(null, new object[] { image, conversion });
            }
            else
            {
                var method = Info.OfMethod("OpenCVSupport", "OpenCVSupport.ImageExtensions", "GetCVMat", "MatrixImage`1");
                method = method.MakeGenericMethod(image.ElementType);
                //var method =
                //    typeof(ImageExtensions).GetMethod(nameof(GetCVMat), 1, new Type[] { typeof(MatrixImage<>) })
                //    .MakeGenericMethod(image.ElementType);
                return (Mat)method.Invoke(null, new object[] { image });
            }
        }

        public static Mat GetCVMat(this IMatrixImage image, bool sync = true)
        {
            var method = Info.OfMethod("OpenCVSupport", "OpenCVSupport.ImageExtensions", "GetCVMat", "MatrixImage`1");
            method = method.MakeGenericMethod(image.ElementType);
            //var method =
            //    typeof(ImageExtensions).GetMethod(nameof(GetCVMat), 1, new Type[] { typeof(MatrixImage<>) })
            //    .MakeGenericMethod(image.ElementType);
            return (Mat)method.Invoke(null, new object[] { image });
        }

        public static Mat<TElement> GetCVMat<TElement>(this MatrixImage<TElement> image)
            where TElement : unmanaged, IComparable<TElement>
        {
            Mat<TElement>[] mats = image.Split(false).Select(a => GetCVMat(a)).ToArray();
            Mat<TElement> mat = new Mat<TElement>(image.Height, image.Width);
            Cv2.Merge(mats, mat);
            return mat;
        }

        public static Mat GetCVMat<TElement>(this MatrixImage<TElement> image, ColorConversionCodes conversion)
            where TElement : unmanaged, IComparable<TElement>
        {
            Mat<TElement>[] mats = image.Split(false).Select(a => GetCVMat(a)).ToArray();
            Mat<TElement> mat = new Mat<TElement>(image.Height, image.Width);
            Cv2.Merge(mats, mat);
            return mat.CvtColor(conversion);
        }

        public static Mat GetCVMat(this IMatrixLayer layer)
        {
            //var method =
            //    typeof(ImageExtensions).GetMethod(nameof(GetCVMat), 1, new Type[] { typeof(MatrixLayer<>) })
            //    .MakeGenericMethod(layer.ElementType);
            var method = Info.OfMethod("OpenCVSupport", "OpenCVSupport.ImageExtensions", "GetCVMat", "MatrixLayer`1");
            method = method.MakeGenericMethod(layer.ElementType);
            return (Mat)method.Invoke(null, new object[] { layer });
        }

        public static Mat<TElement> GetCVMat<TElement>(this MatrixLayer<TElement> layer)
            where TElement : unmanaged, IComparable<TElement>
        {
            Mat<TElement> mat = new Mat<TElement>(layer.Height, layer.Width);
            mat.SetArray(layer.GetStorage(false));
            return mat;
        }

        public static void SetCVMat(this IMatrixImage image, Mat mat)
        {
            //var method =
            //   typeof(ImageExtensions).GetMethod(nameof(SetCVMat), 1, new Type[] { typeof(MatrixImage<>) })
            //   .MakeGenericMethod(image.ElementType);
            var method = Info.OfMethod("OpenCVSupport", "OpenCVSupport.ImageExtensions", "SetCVMat", "MatrixImage`1,Mat");
            method = method.MakeGenericMethod(image.ElementType);
            method.Invoke(null, new object[] { image, mat });
        }

        public static void SetCVMat<TElement>(this MatrixImage<TElement> image, Mat mat)
            where TElement : unmanaged, IComparable<TElement>
        {
            var layers = image.Split(false);
            var mats = mat.Split();
            int min = Math.Min(layers.Length, mats.Length);
            for (int i = 0; i < min; i++)
            {
                layers[i].SetCVMat<TElement>(mats[i]);
            }
        }

        public static void SetCVMat(this IMatrixLayer layer, Mat mat)
        {
            //var method =
            // typeof(ImageExtensions).GetMethod(nameof(SetCVMat), 1, new Type[] { typeof(MatrixLayer<>) })
            // .MakeGenericMethod(layer.ElementType);
            var method = Info.OfMethod("OpenCVSupport", "OpenCVSupport.ImageExtensions", "SetCVMat", "MatrixLayer`1,Mat");
            method = method.MakeGenericMethod(layer.ElementType);
            method.Invoke(null, new object[] { layer, mat });
        }

        public static void SetCVMat<TElement>(this MatrixLayer<TElement> layer, Mat mat)
            where TElement : unmanaged, IComparable<TElement>
        {
            mat.GetArray<TElement>(out TElement[] data);
            layer.SetStorage(data, false);
        }
    }
}