using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NIRS.MethodItems.Preview.Filters
{
    public abstract class AqdOperations : OperationSequence
    {
        public double Aqd { get; }

        public override string GetKey<TElement>() => base.GetKey<TElement>() + $" aqd: {Aqd}";

        protected AqdOperations(int width, int height, double aqd) : base(width, height) => Aqd = aqd;
    }

    [Serializable]
    public class Kaun : ImageMethod
    {
        public int CoreWidth { get; set; }
        public int CoreHeight { get; set; }
        public int Aqd { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            input.SlidingWindow(new KaunOperations(CoreWidth, CoreHeight, Aqd));
            return input;
        }
    }

    public class KaunOperations : AqdOperations
    {
        public KaunOperations(int width, int height, double aqd) : base(width, height, aqd)
        {
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            var array = Expression.Variable(typeElement.MakeArrayType(), "array");
            var avg = Expression.Variable(typeElement, "avg");
            var mid = Expression.Variable(typeElement, "mid");
            //var w = Expression.Variable(typeof(double), "w");
            var c = Expression.Variable(typeof(double), "c");
            var b = Expression.Variable(typeof(double), "b");

            Expression[] expressions = new Expression[] // avg != 0
            {
                ExpCalculateTmpArray(array),            // Подготовить одномерный массив для ядра
                avg.Assign(ExpressionExtends.ArraySum(array, CoreHeight * CoreWidth)
                    .Div(CoreHeight * CoreWidth)),      // Среднее
                mid.Assign(ExpMid()),                   // Центр матрицы
                b.Assign(Aqd),                          // Параметр
                c.Assign(mid.Sub(avg).Div(avg).Abs()),  // c = (mid - avg) / avg
                c.NotEqual(Expression.Default(c.Type))  // return c != 0
                    .Condition(
                        // ? (1.0 - b * b / c) / (b * b + 1.0)
                        //      * src[mid]      .Limit(0, 255)
                        Expression.Constant(1.0).Sub(b.Mul(b).Div(c)).Div(b.Mul(b).Add(1))
                            .Mul(mid).Limit(0, 255).FixType(typeElement),
                        // : (0)
                        Expression.Constant(Expression.Default(typeElement))),
            };

            return Expression.Block(new[] { array, avg, mid, c, b }, expressions);
        }
    }
}