using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NIRS.MethodImpl.Preview.Contours
{
    public static class Implementations
    {
        public static byte LaplasFunc(byte[] a)
        {
            int t = -a[0] - 2 * a[1] - a[2] - 2 * a[3] + 12 * a[4] - 2 * a[5] - a[6] - 2 * a[7] - a[8];
            if (t < 0) return 0;
            else if (t > 255) return 255;
            else return (byte)t;
        }

        public class LaplassOperation : OperationSequence
        {
            public LaplassOperation(int width, int height) : base(width, height)
            {
            }

            protected override Expression CreateExpression(Type typeElement)
            {
                // -a[0] - 2 * a[1] - a[2] - 2 * a[3] + 12 * a[4] - 2 * a[5] - a[6] - 2 * a[7] - a[8];
                //Expression x0y0 = ExpIndex(0, 0);
                //Expression x1y0 = ExpIndex(1, 0).Mul(2);
                //Expression x2y0 = ExpIndex(2, 0);
                //Expression x0y1 = ExpIndex(0, 1).Mul(2);
                //Expression x1y1 = ExpIndex(1, 1).Mul(12);
                //Expression x2y1 = ExpIndex(2, 1).Mul(2);
                //Expression x0y2 = ExpIndex(0, 2);
                //Expression x1y2 = ExpIndex(1, 2).Mul(2);
                //Expression x2y2 = ExpIndex(2, 2);
                //// x1y0 = x1y0.Mul(2); // BinaryOperate(MathOperate.Mul, x1y0, 2);
                //// x0y1 = x0y1.Mul(2);//BinaryOperate(MathOperate.Mul, x0y1, 2);
                //// x1y1 = x1y1.Mul(12);// BinaryOperate(MathOperate.Mul, x1y1, 2);
                //// x2y1 = x2y1.Mul(2);// BinaryOperate(MathOperate.Mul, x2y1, 2);
                //// x1y2 = x1y2.Mul(2); //Expression.Multiply(x1y2, Expression.Constant(2));
                //Expression exp = x0y0.Negate()
                //    .Sub(x1y0)
                //    .Sub(x2y0)
                //    .Sub(x0y1)
                //    .Add(x1y1)
                //    .Sub(x2y1)
                //    .Sub(x0y2)
                //    .Sub(x1y2)
                //    .Sub(x2y2)
                //    .Limit(Expression.Constant(0), Expression.Constant(255));
                //return ExpLambda<TElement>(exp);

                // -a[0,0] - 2 * a[0,1] - a[0,2] - 2 * a[1,0] + 12 * a[1,1] - 2 * a[1,2] - a[2,0] - 2 * a[2,1] - a[2,2];
                var mask = new int[,] { { -1, -2, -1 }, { -2, 12, -2 }, { -1, -2, -1 } };
                Expression exp = ExpResolveMask(mask);
                return exp.Limit(0, 255);
            }
        }

        //public static TElement Test<TElement>(IndexResolver<TElement> indexResolver)
        //{
        //}

        //public static bool UseContourFunc(this IMatrixImage image, out MatrixImage<byte> retImage, Func<byte[], byte> coreFunc, int width, int height, bool alwaysCreateNew = false)
        //{
        //}

        public static void Laplas(this IMatrixImage image)
        {
            var mask = new int[,] {
                { -1, -2, -1 },
                { -2, 12, -2 },
                { -1, -2, -1 }
            };
            image.SlidingWindow(new MatrixOperation<int>(mask, 0, 255));
        }

        //    public static bool Laplas(this IMatrixImage image, out MatrixImage<byte> retImage, bool alwaysCreateNew = false)
        //    {
        //        if (image is MatrixImage<byte> byteImage)
        //        {
        //            byteImage.SlidingWindow(3, 3, true, LaplasFunc);
        //            retImage = byteImage;
        //            return false;
        //        }
        //        else
        //        {
        //            retImage = image.ToByteImage(true);
        //            retImage.SlidingWindow(3, 3, true, LaplasFunc);
        //            return true;
        //        }
        //    }

        //    public static byte RobertsFunc(byte[] a)
        //    {
        //        int gx = a[0] - a[4];
        //        int gy = a[1] + a[3];
        //        double t = Math.Sqrt(gx * gx + gy * gy) + 50;
        //        if (t < 0) return 0;
        //        else if (t > 255) return 255;
        //        else return (byte)t;
        //    }

        //    public static void Roberts(this MatrixImage<byte> image)
        //    {
        //        using (image.SupressUpdating())
        //        {
        //            foreach (var layer in image.SplitWithoutAlpha())
        //                layer.SlidingWindow(3, 3, LaplasFunc);
        //        }
        //    }
        //}

        //public static bool GetRemoveAlpha(this IMatrixImage image, out IMatrixImage? retImage)
        //{
        //    if (image.LayerCount == 4)
        //    {
        //        IMatrixLayer[] layers = image.Split(false);
        //        retImage = MatrixImageBuilder.CreateImage(layers, true);
        //        return true;
        //    }
        //    else
        //    {
        //        retImage = null;
        //        return false;
        //    }
        //}

        //public static bool GetToGray(this IMatrixImage image, out IMatrixImage? retImage)
        //{
        //    if (image.LayerCount == 1)
        //    {
        //        retImage = null;
        //        return false;
        //    }
        //    else
        //    {
        //        IMatrixLayer[] layers = image.Split(false);
        //        layers.Select(a => a.En)
        //            retImage = null;
        //        return false;
        //    }
        //}
    }
}