using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ImageLib.Image
{
    public interface IMatrixLayer : IDisposable, IEnumerable, ICloneable
    {
        Type ElementType { get; }
        int Height { get; }
        int Width { get; }

        Dictionary<string, TagInfo> Tags { get; }

        IMatrixLayer ConvertTo(Type elementType);

        MatrixLayer<byte> ToByteLayer(bool cloneIfSourceByte);

        MatrixLayer<TOtherElement> ConvertTo<TOtherElement>(Func<object, TOtherElement> converter = null) where TOtherElement : unmanaged, IComparable<TOtherElement>;

        IEnumerable<(object value, int x, int y)> EnumerateIndexed();

        IMatrixImage CreateImage();

        int GetIndex(int x, int y);

        object GetValue(int index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetValue(int x, int y) => GetValue(GetIndex(x, y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object GetValue(Utils.ImageUtils.IIndexResolver indexResolver, int x, int y) =>
            GetValue(indexResolver.GetIndex(x, y));

        void Add(IMatrixLayer layer);

        void Add(object value, bool matrixIsLeft = true);

        void Add(object[] vs, bool matrixIsLeft = true, bool useTypeConversion = false);

        void Sub(IMatrixLayer layer);

        void Sub(object value, bool matrixIsLeft = true);

        void Sub(object[] vs, bool matrixIsLeft = true, bool useTypeConversion = false);

        void Mul(object value, bool matrixIsLeft = true);

        void Div(object value, bool matrixIsLeft = true);

        void Mod(object value, bool matrixIsLeft = true);

        /// <summary>
        /// Array[,]
        /// </summary>
        /// <returns>Array[,]</returns>
        Array GetMatrix();

        object GetPoint(int x, int y);

        Array GetStorage(bool copyStorage = true);

        void Insert(IMatrixLayer layer, int xoffset, int yoffset);

        (object min, object max) MinMaxElement();

        ((object value, int x, int y) min, (object value, int x, int y) max) MinMaxElementIndexed();

        void SetPoint(object value, int x, int y);

        void SetStorage(Array storage, bool copyStorage = true);

        Array SubElements(int xstart, int ystart, int width, int height);

        IMatrixLayer SubMatrix(int xstart, int ystart, int width, int height);

        bool IsReadOnly { get; }

        AutoDisposable SupressUpdating();

        AutoDisposable MakeReadOnly(bool autoReleased);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfReadOnly()
        {
            if (IsReadOnly) throw new MatrixReadOnlyException();
        }

        void OnUpdate(Update update, (int x, int y, int width, int height)? rectangleUpdate);

        event EventHandler<UpdateLayer> Updated;

        event EventHandler Disposed;
    }
}