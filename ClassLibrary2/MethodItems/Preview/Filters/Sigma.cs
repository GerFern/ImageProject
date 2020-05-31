using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace NIRS.MethodItems.Preview.Filters
{
    [Serializable]
    public class Sigma : ImageMethod
    {
        public double Aqd { get; set; }
        public int CoreWidth { get; set; }
        public int CoreHeight { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            input.SlidingWindow(new SigmaOperation(CoreWidth, CoreHeight, Aqd));
            //MethodImpl.Preview.Contours.Implementations.Laplas(input);

            //var image = input.ToByteImage(true);
            //foreach (var layer in image.SplitWithoutAlpha())
            //    layer.SlidingWindow(3, 3, Func);
            return input;
        }
    }

    public class SigmaOperation : AqdOperations
    {
        public SigmaOperation(int width, int height, double aqd) : base(width, height, aqd)
        {
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            //size_t top = 0;
            //size_t bottom = 0;
            //for (int k = -lengthSize; k <= lengthSize; k++)
            //    for (int m = -widthSize; m <= widthSize; m++)
            //    {
            //        size_t bb = 0;
            //        if (((1 - 2 * b) * Image[i][j] <= Image[i + k][j + m]) && ((1 + 2 * b) * Image[i][j] >= Image[i + k][j + m]))
            //            bb = 1;
            //        top += Image[i + k][j + m] * bb;
            //        bottom += bb;
            //    }
            //if (bottom != 0)
            //    image[i][j] = RoundTo(top / bottom, 0);
            //else
            //    image[i][j] = 0;

            //var mid = ExpIndex((CoreWidth - 1) / 2, (CoreHeight - 1) / 2).FixType();
            Type type;
            TypeCode tc = Type.GetTypeCode(typeElement);
            if (tc < TypeCode.Int32) type = typeof(int);
            else type = typeElement;
            var mid = Expression.Variable(type, "mid");
            var top = Expression.Variable(mid.Type, "top");
            var bot = Expression.Variable(typeof(int), "bottom");
            var d1 = Expression.Variable(typeof(double), "d1");
            var d2 = Expression.Variable(typeof(double), "d2");
            var current = Expression.Variable(mid.Type, "current");

            List<Expression> expressions = new List<Expression>
            {
                Expression.Assign(mid, ExpIndex((CoreWidth - 1) / 2, (CoreHeight - 1) / 2).FixType()),
                Expression.Assign(d1, mid.Mul(1 - 2 * Aqd)),
                Expression.Assign(d2, mid.Mul(1 + 2 * Aqd)),
            };
            for (int y = 0; y < CoreHeight; y++)
            {
                for (int x = 0; x < CoreWidth; x++)
                {
                    expressions.Add(Expression.Assign(current, ExpIndex(x, y).FixType(current.Type)));
                    expressions.Add(
                    // if (d1 <= current and d2 >= current) { top += current; bot++; }
                    Expression.IfThen(Expression.AndAlso(
                        d1.LessThanOrEqual(current),
                        d2.GreaterThanOrEqual(current)),
                            Expression.Block(new Expression[]
                            {
                                Expression.AddAssign(top, current),
                                Expression.AddAssign(bot, Expression.Constant(1).FixType(bot.Type))
                            })));
                }
            }
            //expressions.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod(nameof(System.Diagnostics.Debug.WriteLine), new[] { typeof(object)}), d1.FixType(typeof(object))));
            //expressions.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod(nameof(System.Diagnostics.Debug.WriteLine), new[] { typeof(object)}), d2.FixType(typeof(object))));
            //expressions.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod(nameof(System.Diagnostics.Debug.WriteLine), new[] { typeof(object)}), bot.FixType(typeof(object))));
            expressions.Add(Expression.Condition(bot.NotEqual(Expression.Constant(0)), top.Div(bot), Expression.Constant(0)));
            return Expression.Block(new[] { mid, top, bot, d1, d2, current }, expressions);
        }
    }
}