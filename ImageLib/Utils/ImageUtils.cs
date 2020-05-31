/// Автор: Лялин Максим ИС-116
/// @2020

using ImageLib.Image;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImageLib.Utils.ImageUtils
{
    public static class Extensions
    {
        public static MatrixLayer<TElement>[] SplitWithoutAlpha<TElement>(this MatrixImage<TElement> image, bool copyLayers = false)
            where TElement : unmanaged
        {
            var layers = image.Split(copyLayers);
            if (layers.Length < 4) return layers;
            else return layers.Take(3).ToArray();
        }

        public static IMatrixLayer[] SplitWithoutAlpha(this IMatrixImage image, bool copyLayers = false)
        {
            var layers = image.Split(copyLayers);
            if (layers.Length < 4) return layers;
            else return layers.Take(3).ToArray();
        }

        public static void SlidingWindow<TElement>(this MatrixImage<TElement> image, OperationSequence operationSequence, bool withoutAlpha = true)
            where TElement : unmanaged
        {
            using (image.SupressUpdating())
            {
                if (withoutAlpha)
                    foreach (var layer in image.SplitWithoutAlpha())
                        layer.SlidingWindow(operationSequence);
                else
                    foreach (var layer in image.Split())
                        layer.SlidingWindow(operationSequence);
            }
        }

        public static void SlidingWindow<TElement>(this MatrixLayer<TElement> layer, OperationSequence operationSequence)
            where TElement : unmanaged
        {
            using (layer.SupressUpdating())
            {
                TElement[] src = layer.GetStorage(false);
                TElement[] dst = layer.GetStorage(true);

                int width = operationSequence.CoreWidth;
                int height = operationSequence.CoreHeight;

                int kernelSize = width * height;
                int imageSize = src.Length;

                int layerWidth = layer.Width;
                int layerHeight = layer.Height;

                int dWidth = width / 2;
                int dHeight = height / 2;

                var operation = operationSequence.GetFunc<TElement>();

                IndexResolver<TElement> indexResolver = new IndexResolver<TElement>(layer, width, height);

                for (int y = 0; y < layerHeight - height; y++)
                {
                    int offset = y * layerWidth;
                    for (int x = 0; x < layerWidth - width; x++)
                    {
                        indexResolver.Offset = offset;
                        dst[x + dWidth + (y + dHeight) * layerWidth] = operation(indexResolver);
                        offset++;
                    }
                }
                layer.SetStorage(dst, false);
            }
        }

        public static void SlidingWindow(this IMatrixImage image, OperationSequence operationSequence, bool withoutAlpha = true)
        {
            //var methodInfo = typeof(Extensions).GetMethod(nameof(SlidingWindow), 1, new[] { typeof(MatrixImage<>), typeof(OperationSequence), typeof(bool) })
            //    .MakeGenericMethod(image.ElementType);
            var methodInfo = typeof(Extensions).GetMethods()
                .Where(a => a.Name == nameof(SlidingWindow))
                .Where(a => a.ContainsGenericParameters)
                .Where(a =>
                {
                    var p = a.GetParameters();
                    return p.Length == 3
                        //&& p[0].ParameterType == typeof(MatrixImage<>)
                        && p[1].ParameterType == typeof(OperationSequence)
                        && p[2].ParameterType == typeof(bool);
                }).First().MakeGenericMethod(image.ElementType);
            methodInfo.Invoke(null, new object[] { image, operationSequence, withoutAlpha });
        }

        public static void SlidingWindow(this IMatrixLayer layer, OperationSequence operationSequence)
        {
            var methodInfo = typeof(Extensions)
                .GetMethod(nameof(SlidingWindow), new[] { typeof(MatrixLayer<>), typeof(OperationSequence) })
                .MakeGenericMethod(layer.ElementType);
            methodInfo.Invoke(null, new object[] { layer, operationSequence });
        }

        public static void SlidingWindow<TElement>(this MatrixImage<TElement> image, int width, int heigth, bool withoutAlpha, Func<TElement[], TElement> func)
            where TElement : unmanaged
        {
            using (image.SupressUpdating())
            {
                if (withoutAlpha)
                    foreach (var layer in image.SplitWithoutAlpha())
                        layer.SlidingWindow(width, heigth, func);
                else
                    foreach (var layer in image.Split())
                        layer.SlidingWindow(width, heigth, func);
            }
        }

        public static void SlidingWindow<TElement>(this MatrixLayer<TElement> layer, int width, int heigth, Func<TElement[], TElement> func)
            where TElement : unmanaged
        {
            TElement[] src = layer.GetStorage(false);
            TElement[] dst = layer.GetStorage(true);

            int kernelSize = width * heigth;
            int imageSize = src.Length;

            int layerWidth = layer.Width;
            int layerHeight = layer.Height;

            int dWidth = width / 2;
            int dHeight = heigth / 2;

            TElement[] window = new TElement[kernelSize];

            for (int y = 0; y < layerHeight - heigth; y++)
            {
                for (int x = 0; x < layerWidth - width; x++)
                {
                    for (int i = 0; i < heigth; i++)
                    {
                        Array.Copy(src, x + (y + i) * layerWidth, window, i * width, width);
                    }
                    dst[x + dWidth + (y + dHeight) * layerWidth] = func(window);
                }
            }

            layer.SetStorage(dst, false);
        }

        public static void SlidingWindow<TElement>(this MatrixLayer<TElement> layer, IndexResolver<TElement> indexResolver, int width, int height, Func<TElement> func)
            where TElement : unmanaged
        {
        }

        public static void SlidingWindow<TElement>(this MatrixLayer<TElement> layer, int width, int height, Func<IndexResolver<TElement>, TElement> func)
           where TElement : unmanaged
        {
        }

        public static void SlidingWindow(this IMatrixLayer layer, int width, int heigth, bool useTypeConversion, Func<IIndexResolver, object> func)
        {
            //TElement[] src = layer.GetStorage(false);
            Array dst = layer.GetStorage(true);

            int kernelSize = width * heigth;
            int imageSize = dst.Length;

            int layerWidth = layer.Width;
            int layerHeight = layer.Height;

            int dWidth = width / 2;
            int dHeight = heigth / 2;

            IIndexResolver indexResolver = IndexResolverBuilder.Create(layer);
            //TElement[] window = new TElement[kernelSize];

            for (int y = 0; y < layerHeight - heigth; y++)
            {
                int offset = y * width;
                for (int x = 0; x < layerWidth - width; x++)
                {
                    //for (int i = 0; i < heigth; i++)
                    //{
                    //    Array.Copy(src, x + (y + i) * layerWidth, window, i * width, width);
                    //}
                    indexResolver.Offset = offset;
                    dst.SetValue(func(indexResolver), x + dWidth + (y + dHeight) * layerWidth);
                    offset++;
                }
            }

            layer.SetStorage(dst, false);
        }
    }

    public class IndexResolverBuilder
    {
        public static IIndexResolver Create(IMatrixLayer layer)
        {
            Type type = typeof(IndexResolver<>).MakeGenericType(layer.ElementType);
            return (IIndexResolver)Activator.CreateInstance(type, layer.Width, layer.GetStorage(false));
        }

        public static IIndexResolver Create(Type elementType, int imageWidth, Array? storage)
        {
            Type type = typeof(IndexResolver<>).MakeGenericType(elementType);
            return (IIndexResolver)Activator.CreateInstance(type, imageWidth, storage);
        }
    }

    public class IndexResolver<TElement> : IIndexResolver where TElement : unmanaged
    {
        private readonly int imageWidth;
        private int offset;
        private int coreWidth;
        private int coreHeight;

        private TElement[] storage;
        private int tmpLen;
        private int lastIndex;
        private int midIndex;
        public readonly TElement[] tmp;
        public TElement[] Tmp => tmp;

        public int Offset
        {
            get => offset;
            set => offset = value;
        }

        public TElement this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        public TElement this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(int x, int y)
        {
            return offset + x + y * imageWidth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(int index)
        {
            return offset + index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int index, TElement value)
        {
            storage[GetIndex(index)] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetValue(int x, int y, TElement value)
        {
            storage[GetIndex(x, y)] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TElement GetValue(int index)
        {
            return storage[GetIndex(index)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TElement GetValue(int x, int y)
        {
            return storage[GetIndex(x, y)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Right()
        {
            offset++;
        }

        public IndexResolver(int imageWidth, TElement[]? storage)
        {
            this.imageWidth = imageWidth;
            this.storage = storage;
        }

        public IndexResolver(int imageWidth, int coreWidth, int coreHeight, TElement[]? storage)
            : this(imageWidth, storage)
        {
            this.coreWidth = coreWidth;
            this.coreHeight = coreHeight;
            tmpLen = coreWidth * coreHeight;
            tmp = new TElement[tmpLen];
            lastIndex = tmpLen - 1;
            midIndex = lastIndex / 2;
        }

        public void CalculateTmp(int width, int height)
        {
            //int maxWidth = x ;
            //int maxHeight = y ;
            int offset = 0;
            for (int j = 0; j < height; j++)
            {
                int tindex = GetIndex(0, j);
                for (int i = 0; i < width; i++)
                {
                    tmp[offset++] = storage[tindex++];
                }
            }
        }

        public void Sort()
        {
            tmp.QuickSort(0, tmpLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TElement[] GetSingleArray() => tmp;

        public TElement Sum()
        {
            TElement sum = default;
            for (int i = 0; i < tmpLen; i++)
            {
                sum = MathUtil.Add(sum, tmp[i]);
            }
            return sum;
        }

        public TElement Avg()
        {
            return MathUtil.DivToInt(Sum(), tmpLen);
        }

        public TElement First()
        {
            return tmp[0];
        }

        public TElement Last()
        {
            return tmp[lastIndex];
        }

        public TElement Mid()
        {
            return tmp[midIndex];
        }

        public TElement TmpFromIndex(int index) => tmp[index];

        public IndexResolver(MatrixLayer<TElement> layer, int coreWidth, int coreHeight) : this(layer.Width, coreWidth, coreHeight, layer.GetStorage(false))
        { }

        object IIndexResolver.this[int index]
        {
            get => GetValue(index);
            set => ((IIndexResolver)this).SetValue(index, value);
        }

        object IIndexResolver.this[int x, int y]
        {
            get => GetValue(x, y);
            set => ((IIndexResolver)this).SetValue(x, y, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IIndexResolver.SetValue(int index, object value) => storage.SetValue(value, GetIndex(index));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IIndexResolver.SetValue(int x, int y, object value) => storage.SetValue(value, GetIndex(x, y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IIndexResolver.GetValue(int index) => GetValue(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IIndexResolver.GetValue(int x, int y) => GetValue(x, y);
    }

    public interface IIndexResolver
    {
        int Offset { get; set; }
        public object this[int index] { get; set; }
        public object this[int x, int y] { get; set; }

        public int GetIndex(int x, int y);

        public int GetIndex(int index);

        public void SetValue(int index, object value);

        public void SetValue(int x, int y, object value);

        public object GetValue(int index);

        public object GetValue(int x, int y);
    }

    //public class Test : OperationSequence
    //{
    //    protected override Expression<Func<IndexResolver<TElement>, TElement>> CreateExpression<TElement>()
    //    {
    //        Expression<Func<IndexResolver<TElement>, TElement>> m00 = ExpGetValueFromIndexResolver<TElement>(0, 0);

    //        return Expression.Lambda<Func<IndexResolver<TElement>, TElement>>(Expression.Add(m00, ExpGetConstant(3)), ExpIndexResolver);
    //    }
    //}

    //public class ExpressionBuilder
    //{
    //    ParameterExpression indexResolver;
    //    int coreWidth, coreHeight;
    //    Dictionary<string, (ParameterExpression variable, bool isInited)> variables = new Dictionary<string, (ParameterExpression, bool)>();
    //    Expression[,] getIndexes;

    //    List<object> expressions = new List<object>();
    //    Expression last;
    //    public ExpressionBuilder Variable(Type type, string name, Expression? initialize)
    //    {
    //        ParameterExpression expVariable = Expression.Variable(type, name);
    //        if (initialize != null)
    //        {
    //            expressions.Add()
    //        }
    //    }

    //    public ExpInner

    //    public ExpressionBuilder(int coreWidth, int coreHeight, ParameterExpression indexResolver)
    //    {
    //        this.coreWidth = coreWidth;
    //        this.coreHeight = coreHeight;
    //        this.indexResolver = indexResolver;
    //    }
    //}

    public abstract class OperationSequence
    {
        private static ConcurrentDictionary<Type, Dictionary<string, (Delegate func, Expression expression)>> hash =
            new ConcurrentDictionary<Type, Dictionary<string, (Delegate, Expression)>>();

        public static void ClearCashe() => hash.Clear();

        protected PropertyInfo IndexerProperty { get; private set; }
        public int CoreWidth { get; }
        public int CoreHeight { get; }

        private readonly Dictionary<string, (Delegate func, Expression expression)> localhash;

        // Func<IndexResolver<TElement>, TElement>
        protected abstract Expression CreateExpression(Type typeElement);

        private ParameterExpression expIndexResolver;

        protected ParameterExpression ExpIndexResolver
        {
            get => expIndexResolver;
            private set
            {
                expIndexResolver = value;
                IndexerProperty = value.Type.GetProperty("Item", new[] { typeof(int), typeof(int) });
            }
        }

        /// <summary>
        /// Получает значение выражения <see cref="IndexResolver{TElement}"/>[<see cref="int"/>]
        /// </summary>
        /// <typeparam name="TElement">Тип элемента</typeparam>
        /// <param name="index">Индекс</param>
        /// <returns></returns>
        protected Expression<Func<IndexResolver<TElement>, TElement>> ExpGetValueFromIndexResolver<TElement>(int index)
             where TElement : unmanaged
        {
            // [index]
            IndexExpression indexEpression = ExpGetIndex(index);
            // IndexResolver[index]
            return Expression.Lambda<Func<IndexResolver<TElement>, TElement>>(indexEpression, expIndexResolver);
        }

        protected Expression<Func<IndexResolver<TElement>, TElement>> ExpGetValueFromIndexResolver<TElement>(int x, int y)
            where TElement : unmanaged
        {
            // [x, y]
            IndexExpression indexEpression = ExpIndex(x, y);
            // IndexResolver[x, y]
            return Expression.Lambda<Func<IndexResolver<TElement>, TElement>>(indexEpression, expIndexResolver);
        }

        //protected bool Optimize(Expression expression, out Expression changed)
        //{
        //    if(expression is )
        //}

        //protected bool Optimize<T>(T expression, out T changed)
        //    where T : Expression
        //{
        //}

        //IndexExpression[,] indexExpressions;
        //Dictionary<IndexExpression, ParameterExpression> dublicate
        //    = new Dictionary<IndexExpression, ParameterExpression>();

        protected Type ElementType { get; private set; }

        protected class IndexVariable
        {
            public int X { get; }
            public int Y { get; }

            public ParameterExpression Parameter { get; set; }
        }

        //protected Expression EnumerateIndexes(
        //    Dictionary<(int,int), ParameterExpression> indexParameters,
        //    ParameterExpression? sum,
        //    ParameterExpression? avg,
        //    bool createArray,
        //    out ParameterExpression? singleArray
        //    )
        //{
        //    List<Expression> expressions = new List<Expression>();
        //    bool createTmpParameter = sum != null && createArray;
        //    ParameterExpression tmp = Expression.Parameter(ElementType);
        //    if (createArray)
        //    {
        //        singleArray = Expression.Variable(ElementType.MakeArrayType());
        //        expressions.Add(singleArray.Assign(Expression.Ne))
        //    }
        //    else singleArray = null;
        //    for (int y = 0; y < CoreHeight; y++)
        //    {
        //        for (int x = 0; x < CoreWidth; x++)
        //        {
        //            Expression indexExpression;
        //            if (indexParameters.TryGetValue((x, y), out ParameterExpression parameterExpression))
        //            {
        //                expressions.Add(parameterExpression.Assign(ExpIndex(x, y)));
        //                indexExpression = parameterExpression;
        //            }
        //            else
        //            {
        //                if(createTmpParameter)
        //            }
        //                indexExpression = ExpIndex(x, y);
        //            if (sum != null)
        //            {
        //                expressions.Add(Expression.AddAssign(sum, indexExpression.FixType(sum.Type)));
        //            }
        //        }
        //    }

        //    if (avg != null)
        //    {
        //        expressions.Add(avg.Assign(sum.Div(CoreHeight * CoreWidth)));
        //    }

        //}

        protected IndexExpression ExpIndex(int x, int y)
        {
            return Expression.Property(expIndexResolver,
                IndexerProperty,
                Expression.Constant(x, typeof(int)),
                Expression.Constant(y, typeof(int)));
            //if (indexExpressions[x, y] == null)
            //{
            //    var indexExp = Expression.Property(expIndexResolver,
            //        IndexerProperty,
            //        Expression.Constant(x, typeof(int)),
            //        Expression.Constant(y, typeof(int)));
            //    indexExpressions[x, y] = indexExp;
            //    return indexExp;
            //}
            //else
            //{
            //    var indexExp = indexExpressions[x, y];
            //    dublicate.ContainsKey()
            //}
        }

        protected IndexExpression ExpGetIndex(int index) =>
            Expression.Property(expIndexResolver,
                IndexerProperty,
                Expression.Constant(index, typeof(int)));

        protected Expression ExpCalculateTmpArray(ParameterExpression? singleArrayParameter)
        {
            Expression call = Expression.Call(expIndexResolver,
                expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.CalculateTmp)),
                Expression.Constant(CoreWidth),
                Expression.Constant(CoreHeight));
            if (singleArrayParameter == null) return call;
            else
            {
                return Expression.Block(
                    call,
                    singleArrayParameter
                        .Assign(Expression.Property(expIndexResolver,
                        expIndexResolver.Type.GetProperty(nameof(IndexResolver<byte>.Tmp)))));
            }
        }

        protected Expression ExpAvg()
        {
            return Expression.Call(expIndexResolver,
                expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.Avg)));
            //.MakeGenericMethod(expIndexResolver.Type.GenericTypeArguments[0]));
        }

        protected Expression ExpSort()
        {
            return Expression.Call(expIndexResolver,
                expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.Sort)));
            //.MakeGenericMethod(expIndexResolver.Type.GenericTypeArguments[0]));
        }

        private Expression[,] indexedExpressions;

        //protected Expression ExpSum(bool fromSingleArray)
        //{
        //    Expression exp;
        //    for (int i = 0; i < length; i++)
        //    {
        //        for (int j = 0; j < length; j++)
        //        {
        //        }
        //    }
        //    //return Expression.Call(expIndexResolver,
        //    //    expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.Sum)));
        //    //.MakeGenericMethod(expIndexResolver.Type.GenericTypeArguments[0]));
        //}

        protected Expression ExpFirst()
        {
            return Expression.Call(expIndexResolver,
                expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.First)));
            //.MakeGenericMethod(expIndexResolver.Type.GenericTypeArguments[0]));
        }

        protected Expression ExpLast()
        {
            return Expression.Call(expIndexResolver,
                expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.Last)));
            //.MakeGenericMethod(expIndexResolver.Type.GenericTypeArguments[0]));
        }

        protected Expression ExpMid()
        {
            return Expression.Call(expIndexResolver,
                expIndexResolver.Type.GetMethod(nameof(IndexResolver<byte>.Mid)));
            //.MakeGenericMethod(expIndexResolver.Type.GenericTypeArguments[0]));
        }

        protected Expression ExpResolveMask<TElement>(TElement[,] mask)
        {
            Expression createExp(Expression preview, TElement value, int x, int y)
            {
                if (preview == null)
                {
                    if (MathUtil.IsDefault(value)) return null;
                    if (MathUtil.IsPositive(value))
                    {
                        if (MathUtil.IsOne(value)) return ExpIndex(x, y);
                        else return ExpIndex(x, y).Mul(Expression.Constant(value));
                    }
                    else
                    {
                        if (MathUtil.IsOne(MathUtil.Negative(value))) return ExpIndex(x, y).Negate();
                        else return ExpIndex(x, y).Mul(Expression.Constant(value));
                    }
                }
                else
                {
                    if (MathUtil.IsDefault(value)) return preview;
                    if (MathUtil.IsPositive(value))
                    {
                        if (MathUtil.IsOne(value)) return preview.Add(ExpIndex(x, y));
                        else return preview.Add(ExpIndex(x, y).Mul(Expression.Constant(value)));
                    }
                    else
                    {
                        if (MathUtil.IsOne(MathUtil.Negative(value))) return preview.Sub(ExpIndex(x, y));
                        else return preview.Sub(ExpIndex(x, y).Mul(Expression.Constant(MathUtil.Negative(value))));
                    }
                }
            }
            int width = mask.GetLength(0);
            int height = mask.GetLength(1);
            Expression expression = null;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    expression = createExp(expression, mask[x, y], x, y);
                }
            }
            return expression ?? Expression.Constant((byte)0);
        }

        protected Expression<Func<IndexResolver<TElement>, TElement>> ExpLambda<TElement>(Expression expression)
            where TElement : unmanaged
        {
            if (expression.Type != typeof(TElement)) expression = Expression.Convert(expression, typeof(TElement));
            return Expression.Lambda<Func<IndexResolver<TElement>, TElement>>(expression, expIndexResolver);
        }

        public Func<IndexResolver<TElement>, TElement> GetFunc<TElement>()
            where TElement : unmanaged
        {
            Func<IndexResolver<TElement>, TElement> func = null;
            string key = GetKey<TElement>();
            if (localhash.TryGetValue(key, out (Delegate func, Expression) value))
            {
                func = value.func as Func<IndexResolver<TElement>, TElement>;
            }
            if (func is null)
            {
                ExpIndexResolver = Expression.Parameter(typeof(IndexResolver<TElement>), "indexResolver");
                Expression<Func<IndexResolver<TElement>, TElement>> lambdaExpression =
                     ExpLambda<TElement>(CreateExpression(typeof(TElement)));
                func = lambdaExpression.Compile();
                localhash.Add(key, (func, lambdaExpression));
            }
            return func;
        }

        public virtual string GetKey<TElement>() => $"t:{typeof(TElement).Name} w:{CoreWidth} h:{CoreHeight}";

        public OperationSequence(int width, int height)
        {
            CoreWidth = width;
            CoreHeight = height;
            if (!hash.TryGetValue(GetType(), out localhash))
            {
                localhash = new Dictionary<string, (Delegate, Expression)>();
                hash.TryAdd(GetType(), localhash);
            }
        }
    }

    public class MatrixOperation<TMatrix> : OperationSequence
        where TMatrix : unmanaged
    {
        private readonly TMatrix[,] matrix;

        public MatrixOperation(TMatrix[,] matrix, TMatrix? minLimit, TMatrix? maxLimit)
            : base(matrix.GetLength(0), matrix.GetLength(1))
        {
            this.matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
            MinLimit = minLimit;
            MaxLimit = maxLimit;
        }

        public TMatrix? MinLimit { get; }
        public TMatrix? MaxLimit { get; }

        public static string MatrixToString(TMatrix[,] m)
        {
            StringBuilder sb = new StringBuilder();
            int width = m.GetLength(0);
            int height = m.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                sb.Append("\n");
                for (int j = 0; j < height; j++)
                {
                    sb.Append(m[i, j]).Append(" ");
                }
            }
            return sb.ToString();
        }

        public override string GetKey<TElement>()
        {
            return base.GetKey<TElement>() + MatrixToString(matrix);
        }

        protected override Expression CreateExpression(Type typeElement)
        {
            return ExpResolveMask(matrix).Limit(MinLimit, MaxLimit);
        }
    }

    public static class ExpressionExtends
    {
        public static UnaryExpression Negate(this Expression expression)
        {
            TypeCode typeCode = Type.GetTypeCode(expression.Type);
            if (typeCode < TypeCode.Int32)
            {
                expression = Expression.Convert(expression, typeof(int));
            }
            else if (typeCode == TypeCode.UInt32)
            {
                expression = Expression.Convert(expression, typeof(long));
            }
            return Expression.Negate(expression);
        }

        public static BinaryExpression BinaryOperate(this Expression expression, object value, MathOperate mathOperate, bool expressionIsLeft = true)
        {
            Expression constant;
            constant = Expression.Constant(value);
            Expression left, right;
            if (expressionIsLeft)
                FixType(expression, constant, out left, out right);
            else
                FixType(expression, constant, out right, out left);

            return BinaryOperate(left, right, mathOperate);

            //if (expression.Type == typeof(int)) constant = Expression.Constant(value);
            //else if (expression.Type == typeof(uint)) constant = Expression.Constant((uint)value);
            //else if (expression.Type == typeof(long)) constant = Expression.Constant((long)value);
            //else if (expression.Type == typeof(ulong)) constant = Expression.Constant((ulong)value);
            //else if (expression.Type == typeof(float)) constant = Expression.Constant((float)value);
            //else if (expression.Type == typeof(double)) constant = Expression.Constant((double)value);
            //else
            //{
            //    constant = Expression.Constant(value);
            //    expression = Expression.Convert(expression, typeof(int));
            //}

            //return mathOperate switch
            //{
            //    MathOperate.Add => expressionIsLeft
            //    ? Expression.Add(expression, constant)
            //    : Expression.Add(constant, expression),
            //    MathOperate.Sub => expressionIsLeft
            //    ? Expression.Subtract(expression, constant)
            //    : Expression.Subtract(constant, expression),
            //    MathOperate.Mul => expressionIsLeft
            //    ? Expression.Multiply(expression, constant)
            //    : Expression.Multiply(constant, expression),
            //    MathOperate.Div => expressionIsLeft
            //    ? Expression.Divide(expression, constant)
            //    : Expression.Divide(constant, expression),
            //    MathOperate.Mod => expressionIsLeft
            //    ? Expression.Modulo(expression, constant)
            //    : Expression.Modulo(constant, expression),
            //    _ => throw new NotSupportedException(),
            //};
        }

        public static BinaryExpression BinaryOperate(this Expression left, Expression right, MathOperate mathOperate)
        {
            Expression fixLeft, fixRight;
            FixType(left, right, out fixLeft, out fixRight);

            return mathOperate switch
            {
                MathOperate.Add => Expression.Add(fixLeft, fixRight),
                MathOperate.Sub => Expression.Subtract(fixLeft, fixRight),
                MathOperate.Mul => Expression.Multiply(fixLeft, fixRight),
                MathOperate.Div => Expression.Divide(fixLeft, fixRight),
                MathOperate.Mod => Expression.Modulo(fixLeft, fixRight),
                _ => throw new NotSupportedException(),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Add(this Expression expression, object value, bool expressionIsLeft = true) => expression.BinaryOperate(value, MathOperate.Add, expressionIsLeft);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Add(this Expression expression, Expression right) => expression.BinaryOperate(right, MathOperate.Add);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Sub(this Expression expression, object value, bool expressionIsLeft = true) => expression.BinaryOperate(value, MathOperate.Sub, expressionIsLeft);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Sub(this Expression expression, Expression right) => expression.BinaryOperate(right, MathOperate.Sub);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Mul(this Expression expression, object value, bool expressionIsLeft = true) => expression.BinaryOperate(value, MathOperate.Mul, expressionIsLeft);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Mul(this Expression expression, Expression right) => expression.BinaryOperate(right, MathOperate.Mul);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Div(this Expression expression, object value, bool expressionIsLeft = true) => expression.BinaryOperate(value, MathOperate.Div, expressionIsLeft);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Div(this Expression expression, Expression right) => expression.BinaryOperate(right, MathOperate.Div);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Mod(this Expression expression, int value, bool expressionIsLeft = true) => expression.BinaryOperate(value, MathOperate.Mod, expressionIsLeft);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BinaryExpression Mod(this Expression expression, Expression right) => expression.BinaryOperate(right, MathOperate.Mod);

        public static BinaryExpression LessThan(this Expression expression, Expression right)
        {
            FixType(expression, right, out Expression fixLeft, out Expression fixRight);
            return Expression.LessThan(fixLeft, fixRight);
        }

        public static BinaryExpression LessThanOrEqual(this Expression expression, Expression right)
        {
            FixType(expression, right, out Expression fixLeft, out Expression fixRight);
            return Expression.LessThanOrEqual(fixLeft, fixRight);
        }

        public static BinaryExpression GreaterThan(this Expression expression, Expression right)
        {
            FixType(expression, right, out Expression fixLeft, out Expression fixRight);
            return Expression.GreaterThan(fixLeft, fixRight);
        }

        public static BinaryExpression GreaterThanOrEqual(this Expression expression, Expression right)
        {
            FixType(expression, right, out Expression fixLeft, out Expression fixRight);
            return Expression.GreaterThanOrEqual(fixLeft, fixRight);
        }

        public static BinaryExpression Equal(this Expression expression, Expression right)
        {
            FixType(expression, right, out Expression fixLeft, out Expression fixRight);
            return Expression.Equal(fixLeft, fixRight);
        }

        public static BinaryExpression NotEqual(this Expression expression, Expression right)
        {
            FixType(expression, right, out Expression fixLeft, out Expression fixRight);
            return Expression.NotEqual(fixLeft, fixRight);
        }

        public static Expression Abs(this Expression expression)
        {
            if (MathUtil.CanBeNegative(expression.Type))
                return Expression.Call(typeof(Math).GetMethod(nameof(Math.Abs), new[] { expression.Type }), expression);
            else return expression;
        }

        public static BinaryExpression Assign(this ParameterExpression expression, Expression right)
        {
            return Expression.Assign(expression, right.FixType(expression.Type));
        }

        public static BinaryExpression Assign(this ParameterExpression expression, object value)
        {
            return Expression.Assign(expression, Expression.Constant(Convert.ChangeType(value, expression.Type)));
        }

        public static ConditionalExpression Condition(this Expression test, Expression ifTrue, Expression ifFalse)
        {
            TypeCode trueCode = Type.GetTypeCode(ifTrue.Type);
            TypeCode falseCode = Type.GetTypeCode(ifFalse.Type);
            if (trueCode < falseCode) ifTrue = ifTrue.FixType(falseCode);
            else if (trueCode > falseCode) ifFalse = ifFalse.FixType(trueCode);
            return Expression.Condition(test.FixType(typeof(bool)), ifTrue, ifFalse);
        }

        public static Expression FixType(this Expression expression, Type type)
        {
            if (expression.Type == typeof(DefaultExpression))
                return Expression.Default(type);
            if (expression.Type == type) return expression;
            //if (expression.Type == typeof(void)) return Expression.Default(type);
            else if (expression is ConstantExpression constantExpression)
                return Expression.Constant(Convert.ChangeType(constantExpression.Value, type));
            else return Expression.Convert(expression, type);
        }

        public static Expression FixType(this Expression expression)
        {
            var typeCode = Type.GetTypeCode(expression.Type);
            if (typeCode < TypeCode.Int32) return expression.FixType(typeof(int));
            else return expression;
        }

        public static void FixType(Expression left, Expression right, out Expression fixLeft, out Expression fixRight)
        {
            TypeCode minimumType = TypeCode.Int32;
            if (left.Type == typeof(uint)) minimumType = TypeCode.UInt32;
            else if (left.Type == typeof(long)) minimumType = TypeCode.Int64;
            else if (left.Type == typeof(ulong)) minimumType = TypeCode.UInt64;
            else if (left.Type == typeof(float)) minimumType = TypeCode.Single;
            else if (left.Type == typeof(double)) minimumType = TypeCode.Double;

            if (right.Type == typeof(uint))
            {
                if (minimumType < TypeCode.UInt32)
                    minimumType = TypeCode.UInt32;
            }
            else if (right.Type == typeof(long))
            {
                if (minimumType < TypeCode.Int64)
                    minimumType = TypeCode.Int64;
            }
            else if (right.Type == typeof(ulong))
            {
                if (minimumType < TypeCode.UInt64)
                    minimumType = TypeCode.UInt64;
            }
            else if (right.Type == typeof(float))
            {
                if (minimumType < TypeCode.Single)
                    minimumType = TypeCode.Single;
            }
            else if (right.Type == typeof(double)) minimumType = TypeCode.Double;
            Type type = minimumType switch
            {
                TypeCode.Int32 => typeof(int),
                TypeCode.UInt32 => typeof(uint),
                TypeCode.Int64 => typeof(long),
                TypeCode.UInt64 => typeof(ulong),
                TypeCode.Single => typeof(float),
                TypeCode.Double => typeof(double),
                _ => throw new NotSupportedException(),
            };
            fixLeft = left.FixType(type);
            fixRight = right.FixType(type);
            //fixLeft = left.Type == type ? left : Expression.Convert(left, type);
            //fixRight = right.Type == type ? right : Expression.Convert(right, type);
        }

        public static void FixType(ref Expression left, ref Expression right)
        {
            FixType(left, right, out left, out right);
        }

        public static Expression FixType(this Expression expression, TypeCode typeCode)
        {
            if (Type.GetTypeCode(expression.Type) == typeCode) return expression;
            else
            {
                Type type = typeCode switch
                {
                    TypeCode.Boolean => typeof(bool),
                    TypeCode.Byte => typeof(byte),
                    TypeCode.SByte => typeof(sbyte),
                    TypeCode.Int16 => typeof(short),
                    TypeCode.UInt16 => typeof(ushort),
                    TypeCode.Int32 => typeof(int),
                    TypeCode.UInt32 => typeof(uint),
                    TypeCode.Int64 => typeof(long),
                    TypeCode.UInt64 => typeof(ulong),
                    TypeCode.Single => typeof(float),
                    TypeCode.Double => typeof(double),
                    _ => throw new NotSupportedException(),
                };
                return expression.FixType(type);
                //return Expression.Convert(expression, type);
            }
        }

        public static Type FixType(Type type)
        {
            return GetTypeFromTypeCode(GetMinTypeCode(new[] { Type.GetTypeCode(type), TypeCode.Int32 }));
        }

        public static TypeCode GetMinTypeCode(IEnumerable<TypeCode> expressions)
        {
            return expressions.Max();
        }

        public static TypeCode GetMinTypeCode(IEnumerable<Expression> expressions)
        {
            return expressions.Select(a => Type.GetTypeCode(a.Type)).Concat(new[] { TypeCode.Int32 }).Max();
        }

        public static Expression Limit(this Expression expression, object? min, object? max) =>
            expression.Limit(min == null ? null : Expression.Constant(min),
                max == null ? null : Expression.Constant(max));

        public static Expression Limit(this Expression expression, Expression? min, Expression? max)
        {
            TypeCode typeCode = GetMinTypeCode(new[] { expression, min, max });
            expression = FixType(expression, typeCode);
            min = FixType(min, typeCode);
            max = FixType(max, typeCode);

            ParameterExpression variable = Expression.Variable(expression.Type);

            ConditionalExpression condition;
            if (min != null && max != null)
                condition = Expression.Condition(Expression.LessThan(variable, min), min,
                Expression.Condition(Expression.GreaterThan(variable, max), max, variable));
            else if (min == null && max != null)
                condition = Expression.Condition(Expression.GreaterThan(variable, max), max, variable);
            else if (min != null && max == null)
                condition = Expression.Condition(Expression.LessThan(variable, min), min, variable);
            else return expression;
            //variable = Expression.Assign(variable, expression);
            //Expression condition = Expression.IfThenElse(Expression.LessThan(variable, min), Expression.Assign(variable, min),
            //    Expression.IfThen(Expression.GreaterThan(variable, max), Expression.Assign(variable, max)));
            BlockExpression block = Expression.Block(new[] { variable }, new Expression[] { Expression.Assign(variable, expression),
                //Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new[]{ variable.Type}), Expression.Convert(variable, typeof(object))),
                condition});

            //Expression invoke = Expression.Invoke(block, expression);
            return block;
            // if(expression < min) return min;
            // else if(expression > max) return max;
            // else return expression
        }

        public static Type GetTypeFromTypeCode(TypeCode typeCode)
        {
            return typeCode switch
            {
                TypeCode.Boolean => typeof(bool),
                TypeCode.Byte => typeof(byte),
                TypeCode.SByte => typeof(sbyte),
                TypeCode.Int16 => typeof(short),
                TypeCode.UInt16 => typeof(ushort),
                TypeCode.Int32 => typeof(int),
                TypeCode.UInt32 => typeof(uint),
                TypeCode.Int64 => typeof(long),
                TypeCode.UInt64 => typeof(ulong),
                TypeCode.Single => typeof(float),
                TypeCode.Double => typeof(double),
                _ => throw new NotSupportedException(),
            };
        }

        public static BlockExpression ArraySum(Expression array, int? length)
        {
            if (!array.Type.IsArray) throw new Exception("Not Array");
            Type elementType = GetTypeFromTypeCode(GetMinTypeCode(new[]
            {
                Type.GetTypeCode(array.Type.GetElementType()),
                TypeCode.Int32
            }));
            //PropertyInfo indexProperty = array.Type.GetProperty("Item");
            if (length != null)
            {
                var sum = Expression.Variable(elementType);
                List<Expression> expressions = new List<Expression>();
                for (int i = 0; i < length; i++)
                {
                    expressions.Add(sum.Assign(sum.Add(Expression.ArrayIndex(array, Expression.Constant(i)))));
                }

                return Expression.Block(new[] { sum }, expressions);
            }
            else
            {
                PropertyInfo lenProp = array.Type.GetProperty(nameof(Array.Length));
                var sum = Expression.Variable(elementType);
                var index = Expression.Variable(typeof(int));
                var arrLen = Expression.Variable(typeof(int));
                var label = Expression.Label(typeof(int));
                return Expression.Block(new[] { sum, index, arrLen }, new Expression[]
                    {
                Expression.Assign(arrLen, Expression.Property(array, lenProp)),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(Expression.Increment(index), arrLen),
                        Expression.AddAssign(sum, Expression.ArrayIndex(array, index)),
                        Expression.Break(label, sum)))
                    });
            }
        }

        public static MethodCallExpression SortArray(Expression array, int? min, int? max)
        {
            MethodInfo sort = typeof(MathUtil)
                .GetMethod(nameof(MathUtil.QuickSort))
                .MakeGenericMethod(array.Type.GetElementType());

            Expression expMin, expMax;

            expMin = Expression.Constant(min ?? 0);
            if (max == null)
            {
                PropertyInfo lenProp = array.Type.GetProperty(nameof(Array.Length));
                expMax = Expression.Property(array, lenProp).Sub(1);
            }
            else expMax = Expression.Constant(max.Value);

            return Expression.Call(sort, array, expMin, expMax);
        }

        public static Expression DebugLog(this Expression expression, string premessage = null)
        {
            if (premessage == null) premessage = string.Empty;

            Expression str = Expression.Call(expression, expression.Type.GetMethod("ToString", new Type[] { }));
            return Expression.Call(typeof(System.Diagnostics.Debug)
                .GetMethod(nameof(System.Diagnostics.Debug.WriteLine), new[] { typeof(string) }),
                ConcatStrings(Expression.Constant(premessage), str));
        }

        public static Expression ConcatStrings(Expression str1, Expression str2)
        {
            str1 = str1.FixType(typeof(string));
            str2 = str2.FixType(typeof(string));
            return Expression.Call(typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }), str1, str2);
        }
    }

    public enum MinimumType
    {
        Int32,
        UInt32,
        Int64,
        UInt64,
        Single,
        Double
    }

    public enum MathOperate
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod
    }
}