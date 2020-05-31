/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;

using System;
using System.Linq.Expressions;

namespace NIRS.MethodItems.Preview.Contours
{
    [Serializable]
    internal class Roberts : ImageMethod
    {
        public static byte Func(byte[] a)
        {
            int gx = a[0] - a[4];
            int gy = a[1] + a[3];
            double t = Math.Sqrt(gx * gx + gy * gy) + 50;
            if (t < 0) return 0;
            else if (t > 255) return 255;
            else return (byte)t;
        }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            double[,] x = new double[,]
            {
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            };
            double[,] y = new double[,]
            {
                { 0, 0, 0 },
                { 0, 0, 1 },
                { 0, 1, 0 }
            };
            input.SlidingWindow(new GxGyOperation(3, 3, x, y));
            //input.SlidingWindow(new RobertsOperation(3, 3), true);
            return input;
            //var image = input.ToByteImage(true);
            //foreach (var layer in image.SplitWithoutAlpha())
            //    layer.SlidingWindow(3, 3, Func);
            //return image;
        }
    }

    public class RobertsOperation : OperationSequence
    {
        public RobertsOperation(int width, int height) : base(width, height)
        {
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            // -a[0] - 2 * a[1] - a[2] - 2 * a[3] + 12 * a[4] - 2 * a[5] - a[6] - 2 * a[7] - a[8];
            //int gx = a[0] - a[4];
            //int gy = a[1] + a[3];

            Expression gx = ExpIndex(0, 0).Sub(ExpIndex(1, 1)).FixType(typeof(double));
            Expression gy = ExpIndex(1, 0).Add(ExpIndex(0, 1)).FixType(typeof(double));
            gx = Expression.Power(gx, Expression.Constant(2.0));
            gy = Expression.Power(gy, Expression.Constant(2.0));
            //gx = Expression.Convert(gx, typeof(double));
            //gy = Expression.Convert(gy, typeof(double));
            Expression exp = Expression.Call(typeof(Math).GetMethod(nameof(Math.Sqrt)), gx.Add(gy)).Add(50);

            return exp.Limit(0, 255);

            //return ExpLambda<TElement>(exp);
        }
    }
}