using ImageLib;
using ImageLib.Image;
using ImageLib.Utils.ImageUtils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace NIRS.MethodItems.Preview.Filters
{
    [Serializable]
    public class Lee : ImageMethod
    {
        public double Aqd { get; set; }
        public int CoreWidth { get; set; }
        public int CoreHeight { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            input.SlidingWindow(new LeeOperation(CoreWidth, CoreHeight, Aqd));
            return input;
        }
    }

    public class LeeOperation : AqdOperations
    {
        public LeeOperation(int width, int height, double aqd) : base(width, height, aqd)
        {
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            //size_t average = 0;
            //for (int k = -lengthSize; k <= lengthSize; k++)
            //    for (int m = -widthSize; m <= widthSize; m++)
            //        average += Image[i + k][j + m];
            //average /= size;
            //double W = 0.0;
            //double top, bottom;
            //top = (double)((Image[i][j] - average) * (Image[i][j] - average)) - (double)(b * b * average * average);
            //bottom = (double)((Image[i][j] - average) * (Image[i][j] - average)) - (double)(b * b * b * b * average * average);
            //if (bottom != 0)
            //    W = top / bottom;
            ////	W = 1;
            //if (RoundTo(Image[i][j] * W, 0) + RoundTo(average * (1 - W), 0) >= 0 &&
            //RoundTo(Image[i][j] * W, 0) + RoundTo(average * (1 - W), 0) <= 255)
            //    image[i][j] = RoundTo(Image[i][j] * W, 0) + RoundTo(average * (1 - W), 0);
            //else if (RoundTo(Image[i][j] * W, 0) + RoundTo(average * (1 - W), 0) < 0)
            //    image[i][j] = 0;
            //else
            //    image[i][j] = 255;

            Type type;
            TypeCode tc = Type.GetTypeCode(typeElement);
            if (tc < TypeCode.Int32) type = typeof(int);
            else type = typeElement;

            var array = Expression.Variable(typeElement.MakeArrayType(), "array");
            var avg = Expression.Variable(type, "avg");
            var avgx = Expression.Variable(type, "avgMulAvg");
            var w = Expression.Variable(typeof(double), "w");
            var top = Expression.Variable(typeof(double), "top");
            var bot = Expression.Variable(typeof(double), "bottom");
            var mid = Expression.Variable(type, "mid");
            var midSubAvg = Expression.Variable(type, "midSubAvg");
            var midSubAvgx = Expression.Variable(typeof(double), "midSubAvgx");
            var b = Expression.Variable(typeof(double), "aqd");
            var bb = Expression.Variable(typeof(double), "aqdMulAqd");

            BlockExpression blockExpression;
            ConditionalExpression conditionalExpression;

            List<Expression> expressions = new List<Expression>
            {
                ExpCalculateTmpArray(array),
                // avg = Средняя арифмитическое всех элементов окна;
                Expression.Assign(avg, ExpressionExtends.ArraySum(array, CoreWidth * CoreHeight)
                    .Div(CoreWidth * CoreHeight).FixType(type)),
                Expression.Assign(avgx, avg.Mul(avg)),
                Expression.Assign(b, Expression.Constant(Aqd)),
                Expression.Assign(bb, b.Mul(b)),
                // mid = центральный элемент окна;
                Expression.Assign(mid, ExpMid().FixType(type)),
                Expression.Assign(midSubAvg, mid.Sub(avg)),
                Expression.Assign(midSubAvgx, midSubAvg.Mul(midSubAvg).FixType(typeof(double))),
                // top = ((mid - avg) * (mid - avg)) - (aqd * aqd * avg * avg);
                Expression.Assign(top, midSubAvgx.Sub(bb.Mul(avgx))),
                // bot = ((mid - avg) * (mid - avg)) - (aqd ^ 4 * avg ^ 4);
                Expression.Assign(bot, midSubAvgx.Sub(bb.Mul(bb).Mul(avgx).Mul(avgx))),
                // w = bot != 0 ? top / bot : 0;
                Expression.Assign(w, Expression.Condition(bot.NotEqual(Expression.Constant(0.0)),
                    top.Div(bot), Expression.Constant(0.0))),
                // return (mid * w) + mid * (1 - w);
                mid.Mul(w).Add(mid.Mul(Expression.Constant(1.0).Sub(w))).Limit(0, 255)
            };

            return Expression.Block(new[] { array, avg, avgx, w, top, bot, mid, midSubAvg, midSubAvgx, b, bb }, expressions);

            //expressions.Add(Expression.Condition(bot.NotEqual(Expression.Constant(0)), top.Div(bot), Expression.Constant(0)));
            //return Expression.Block(new[] { mid, top, bot, d1, d2, current }, expressions);
        }
    }
}