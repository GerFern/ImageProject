using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImageLib.Image
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public interface IMatrixImage : IEnumerable<IMatrixLayer>, IDisposable, ICloneable
    {
        Type ElementType { get; }
        int Height { get; }
        int LayerCount { get; }
        int Width { get; }
        bool IsDisposed { get; }
        Dictionary<string, TagInfo> Tags { get; }

        IMatrixImage ConvertTo(Type elementType);

        MatrixImage<TOtherElement> ConvertTo<TOtherElement>() where TOtherElement : unmanaged, IComparable<TOtherElement>;

        MatrixImage<byte> ToByteImage(bool cloneIfSourceByte);

        IMatrixLayer CreateSingleGray();

        IMatrixLayer GetLayer(int indexLayer, bool copyLayer = true);

        void Insert(IMatrixImage image, int xoffset, int yoffset);

        void SetLayer(IMatrixLayer layer, int indexLayer, bool copyLayer = true);

        IMatrixLayer[] Split(bool copyLayers);

        IMatrixImage SubImage(int xstart, int ystart, int width, int height);

        /// <summary>
        /// Перебор по каждому пикселю
        /// </summary>
        /// <param name="action">Действие перебора для каждого пикселя</param>
        public void ForEachPixels(Action<object[]> action);

        /// <summary>
        /// Перебор по каждому пикселю с координатами
        /// </summary>
        /// <param name="action">Действие перебора с координатами x, y для каждого пикселя</param>
        public void ForEachPixels(Action<object[], int, int> action);

        /// <summary>
        /// Установка значений для каждого пикселя
        /// </summary>
        /// <param name="func">Метод установки для каждого пикселя</param>
        public void ForEachPixelsSet(Func<object[]> func);

        /// <summary>
        /// Установка значений для каждого пикселя на основе исходного значения
        /// </summary>
        /// <param name="func">Метод установки для каждого пикселя на основе исходного значения</param>
        public void ForEachPixelsSet(Func<object[], object[]> func);

        /// <summary>
        /// Установка значений для каждого пикселя с координатами
        /// </summary>
        /// <param name="action">Метод установки с координатами x, y для каждого пикселя</param>
        public void ForEachPixelsSet(Func<int, int, object[]> func);

        /// <summary>
        /// Установка значений для каждого пикселя на основе исходного значения с координатами
        /// </summary>
        /// <param name="func">Метод установки для каждого пикселя на основе исходного значения с координатами x, y</param>
        public void ForEachPixelsSet(Func<object[], int, int, object[]> func);

        bool IsReadOnly { get; }

        AutoDisposable MakeReadOnly(bool auteRelease, bool lockLayers);

        AutoDisposable SupressUpdating();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ThrowIfReadOnly()
        {
            if (IsReadOnly) throw new MatrixReadOnlyException();
        }

        void OnUpdate(Update update, (int x, int y, int width, int height)? rectangleUpdate);

        event EventHandler<UpdateImage> Updated;

        event EventHandler Disposed;
    }

    public class UpdateImage
    {
        public UpdateImage(UpdateLayer singleLayerUpdate, IMatrixImage image, Update update, (int x, int y, int width, int height)? rectangleUpdate)
        {
            SingleLayerUpdate = singleLayerUpdate;
            Image = image ?? throw new ArgumentNullException(nameof(image));
            Update = update;
            RectangleUpdate = rectangleUpdate;
        }

        public UpdateLayer? SingleLayerUpdate { get; }
        public IMatrixImage Image { get; }
        public Update Update { get; }
        private (int x, int y, int width, int height)? RectangleUpdate { get; }
    }

    public class UpdateLayer
    {
        public UpdateLayer(IMatrixLayer image, Update update, (int x, int y, int width, int height) rectangleUpdate)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            Update = update;
            RectangleUpdate = rectangleUpdate;
        }

        public IMatrixLayer Image { get; }
        public Update Update { get; }
        public (int x, int y, int width, int height) RectangleUpdate { get; }
    }

    public enum Update
    {
        pixel,
        rectangle,
        Full
    }
}