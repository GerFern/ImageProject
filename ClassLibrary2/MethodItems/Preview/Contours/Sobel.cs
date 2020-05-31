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
    internal class Sobel : ImageMethod
    {
        public static byte Func(byte[] a)
        {
            int gx = a[0] + 2 * a[1] + a[2] - (a[6] + 2 * a[7] + a[8]);
            int gy = a[0] + 2 * a[3] + a[6] - (a[2] + 2 * a[5] + a[8]);
            double t = Math.Sqrt(gx * gx + gy * gy);
            if (t < 0) return 0;
            else if (t > 255) return 255;
            else return (byte)t;
        }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            double[,] x = new double[,] {
                { 1, 2, 1 },
                { 0, 0, 0 },
                { -1, -2, -1 }
            };
            double[,] y = new double[,] {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 }
            };
            input.SlidingWindow(new GxGyOperation(3, 3, x, y));
            return input;
            //MatrixImage<byte> image = input.ToByteImage(true);
            //foreach (var layer in image.SplitWithoutAlpha())
            //    layer.SlidingWindow(3, 3, Func);
            //return image;
        }
    }

    public class GxGyOperation : OperationSequence
    {
        private double[,] vs_x;
        private double[,] vs_y;

        public override string GetKey<TElement>()
        {
            return base.GetKey<TElement>() +
                "vs_x: " + MatrixOperation<double>.MatrixToString(vs_x) +
                "vs_y: " + MatrixOperation<double>.MatrixToString(vs_y);
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            Expression gx = FindG(vs_x).FixType(typeof(double));
            Expression gy = FindG(vs_y).FixType(typeof(double));
            gx = Expression.Call(typeof(Math).GetMethod(nameof(Math.Pow)), gx, Expression.Constant(2.0d));
            gy = Expression.Call(typeof(Math).GetMethod(nameof(Math.Pow)), gy, Expression.Constant(2.0d));
            Expression exp = Expression.Call(typeof(Math).GetMethod(nameof(Math.Sqrt)), gx.Add(gy));
            return exp.Limit(0, 255);
        }

        private Expression FindG(double[,] vs)
        {
            Func<Expression, int, int, Expression> func = (prev, x, y) =>
            {
                var value = vs[x, y];
                if (value == 0) return prev;
                if (prev != null)
                {
                    if (value == 1) return prev.Add(ExpIndex(x, y));
                    if (value == -1) return prev.Sub(ExpIndex(x, y));
                    if (value > 0) return prev.Add(ExpIndex(x, y).Mul(value));
                    return prev.Sub(ExpIndex(x, y).Mul(-value));
                }
                else
                {
                    if (value == 1) return ExpIndex(x, y);
                    if (value == -1) return ExpIndex(x, y).Negate();
                    if (value > 0) return ExpIndex(x, y).Mul(value);
                    return ExpIndex(x, y).Mul(-value);
                }
            };
            Expression expression = null;
            for (int i = 0; i < vs.GetLength(0); i++)
            {
                for (int j = 0; j < vs.GetLength(1); j++)
                {
                    expression = func(expression, i, j);
                }
            }

            return expression ?? Expression.Default(ElementType);
        }

        public GxGyOperation(int coreWidth, int coreHeight, double[,] vs_x, double[,] vs_y) : base(coreWidth, coreHeight)
        {
            this.vs_x = vs_x;
            this.vs_y = vs_y;
        }
    }
}