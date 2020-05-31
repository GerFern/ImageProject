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
    public class Average : ImageMethod
    {
        public int CoreWidth { get; set; }
        public int CoreHeight { get; set; }

        public override IMatrixImage Invoke(IMatrixImage input)
        {
            input.SlidingWindow(new AverageOperation(CoreWidth, CoreHeight));
            return input;
        }
    }

    public class AverageOperation : OperationSequence
    {
        public AverageOperation(int width, int height) : base(width, height)
        {
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            var array = Expression.Variable(typeElement.MakeArrayType(), "array");
            return Expression.Block(new[] { array }, new Expression[]
            {
                ExpCalculateTmpArray(array),        // Подготовить одномерный массив
                ExpressionExtends.ArraySum(array, CoreWidth * CoreHeight)
                    .Div(CoreWidth * CoreHeight)    // Найти среднее
            });                                     // Последнее выражение блока будет результатом окна фильтра
        }
    }
}