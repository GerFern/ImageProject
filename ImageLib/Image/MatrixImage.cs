/// Автор: Лялин Максим ИС-116
/// @2020

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using ImageLib.Utils.ImageUtils;
using System.ComponentModel;
using ImageLib.Utils;
using System.Collections.Immutable;
using Avalonia.Media.Imaging;
using ReactiveUI.Fody.Helpers;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Collections;
using ImageLib.Model.Drawing;
using ImageLib.Controls;
using ImageLib.Model;

namespace ImageLib.Image
{

    /// <summary>
    /// Матричное изображение
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    [View(typeof(ImageRender))]
    public class MatrixImage<TElement> : ISerializable, IEnumerable<MatrixLayer<TElement>>, ICloneable, IMatrixImage where TElement : unmanaged/*, IComparable<TElement>*/
    {
        private readonly MatrixLayer<TElement>[] layers;
        public Type ElementType => typeof(TElement);
        public int Width { get; }
        public int Height { get; }
        public int LayerCount => layers.Length;
        public ImmutableArray<MatrixLayer<TElement>> Layers { get; }
        public AvaloniaList<DrawModel> DrawModels { get; }
            = new AvaloniaList<DrawModel>();
        public Dictionary<string, TagInfo> Tags { get; }
        = new Dictionary<string, TagInfo>();

        [NonSerialized]
        private HashSet<AutoDisposable> rokeys = new HashSet<AutoDisposable>();

        [NonSerialized]
        private HashSet<AutoDisposable> sukeys = new HashSet<AutoDisposable>();

        public event EventHandler<UpdateImage> Updated;

        public event EventHandler Disposed;

        public bool IsReadOnly => rokeys.Count > 0;

        public AutoDisposable SupressUpdating()
        {
            AutoDisposable key = new AutoDisposable() { AutoRelease = true };
            sukeys.Add(key);
            var layersSupressUpdating = layers.Select(a => a.SupressUpdating()).ToArray();
            void release()
            {
                sukeys.Remove(key);
                if (sukeys.Count == 0) OnUpdate(Update.Full, null);
                key.Released -= release;

                foreach (var item in layersSupressUpdating)
                    item.Dispose();
            }
            key.Released += release;
            return key;
        }

        public AutoDisposable MakeReadOnly(bool autoReleased = false, bool lockLayers = true)
        {
            AutoDisposable key = new AutoDisposable();
            rokeys.Add(key);
            AutoDisposable[] layersKey = new AutoDisposable[layers.Length];
            if (lockLayers)
                for (int i = 0; i < layers.Length; i++)
                    layersKey[i] = layers[i].MakeReadOnly(autoReleased);

            void release()
            {
                rokeys.Remove(key);
                foreach (var item in layersKey)
                    item.Dispose();
            }
            key.Released += release;
            return key;
        }

        public MatrixImage<byte> ToByteImage(bool cloneIfSourceByte = false)
        {
            if (this.ElementType == typeof(byte) && !cloneIfSourceByte)
            {
                var t = this;
                return Unsafe.As<MatrixImage<TElement>, MatrixImage<byte>>(ref t);
            }
            else return ConvertTo<byte>();
        }

        protected virtual void OnUpdate(UpdateImage update)
        {
            if (!supressUpdate)
            {
                UpdateBitmap();
                Updated?.Invoke(this, update);
            }
        }

        protected virtual void OnUpdate(UpdateLayer update)
        {
            if (!supressUpdate)
            {
                UpdateBitmap();
                OnUpdate(new UpdateImage(update, this, update.Update, update.RectangleUpdate));
            }
        }

        protected void OnUpdate(object _, UpdateLayer update) => OnUpdate(update);

        public virtual void OnUpdate(Update update, (int x, int y, int width, int height)? rectangleUpdate)
        {
            if (!supressUpdate)
            {
                OnUpdate(new UpdateImage(null, this, update, rectangleUpdate));
            }
        }

        protected bool supressUpdate = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckReadOnly()
        {
            if (IsReadOnly) throw new MatrixReadOnlyException();
        }

        public MatrixLayer<TElement>[] Split(bool copyLayers = true)
        {
            MatrixLayer<TElement>[] split = new MatrixLayer<TElement>[layers.Length];
            if (copyLayers)
                for (int i = 0; i < layers.Length; i++)
                    split[i] = layers[i].Clone();
            else
                for (int i = 0; i < layers.Length; i++)
                    split[i] = layers[i];
            return split;
        }

        public MatrixImage(int width, int height) : this(width, height, 1)
        { }

        public MatrixImage(int width, int height, int layersCount) : this()
        {
            Width = width;
            Height = height;
            layers = new MatrixLayer<TElement>[layersCount];
            for (int i = 0; i < layersCount; i++)
            {
                layers[i] = new MatrixLayer<TElement>(width, height);
                layers[i].Disposed += LayerDisposed;
                layers[i].Updated += OnUpdate;
            }
        }

        public MatrixImage(int width, int height, int layersCount, TElement defaultValue) : this()
        {
            Width = width;
            Height = height;
            layers = new MatrixLayer<TElement>[layersCount];
            for (int i = 0; i < layersCount; i++)
            {
                layers[i] = new MatrixLayer<TElement>(width, height, defaultValue);
                layers[i].Disposed += LayerDisposed;
                layers[i].Updated += OnUpdate;
            }
        }

        public MatrixImage(MatrixLayer<TElement>[] layers, bool copyLayers = true) : this()
        {
            int width = layers[0].Width;
            int height = layers[0].Height;
            this.layers = new MatrixLayer<TElement>[layers.Length];
            //TODO : Проверки на размерность слоев
            if (copyLayers)
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i].Width != width || layers[i].Height != height) throw new Exception();
                    this.layers[i] = layers[i].Clone();
                }
            else
                for (int i = 0; i < layers.Length; i++)
                {
                    if (layers[i].Width != width || layers[i].Height != height) throw new Exception();
                    this.layers[i] = layers[i];
                }
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Disposed += LayerDisposed;
                layers[i].Updated += OnUpdate;
            }
            Width = width;
            Height = height;
        }

        public MatrixImage(int width, int height, IEnumerable<TElement>[] layers) : this()
        {
            Width = width;
            Height = height;
            this.layers = new MatrixLayer<TElement>[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                this.layers[i] = new MatrixLayer<TElement>(width, height, layers[i]);
                this.layers[i].Disposed += LayerDisposed;
                this.layers[i].Updated += OnUpdate;
            }
        }

        public MatrixImage(TElement[][,] matrixLayers) : this()
        {
            int width = matrixLayers[0].GetLength(0);
            int height = matrixLayers[0].GetLength(1);
            layers = new MatrixLayer<TElement>[matrixLayers.Length];
            for (int i = 0; i < matrixLayers.Length; i++)
            {
                if (width != matrixLayers[i].GetLength(0) && height != matrixLayers[i].GetLength(1)) throw new Exception();
                layers[i] = new MatrixLayer<TElement>(matrixLayers[i]);
                layers[i].Disposed += LayerDisposed;
                layers[i].Updated += OnUpdate;
            }
        }

        public static IEnumerable<T[]> ConcatEnumerables<T>(params IEnumerable<T>[] enumerables)
        {
            var en = enumerables.Select(a => a.GetEnumerator()).ToArray();
            while (en.All(a => a.MoveNext()))
            {
                yield return en.Select(a => a.Current).ToArray();
            }
            yield break;
        }

        /// <summary>
        /// Перебор по каждому пикселю
        /// </summary>
        /// <param name="action">Действие перебора для каждого пикселя</param>
        public void ForEachPixels(Action<TElement[]> action)
        {
            foreach (var item in ConcatEnumerables(layers))
                action(item);
        }

        /// <summary>
        /// Перебор по каждому пикселю с координатами
        /// </summary>
        /// <param name="action">Действие перебора с координатами x, y для каждого пикселя</param>
        public void ForEachPixels(Action<TElement[], int, int> action)
        {
            int x = 0, y = 0;
            foreach (var item in ConcatEnumerables(layers))
            {
                if (x >= Width)
                {
                    x = 0;
                    y++;
                }
                action(item, x, y);
                x++;
            }
        }

        /// <summary>
        /// Установка значений для каждого пикселя
        /// </summary>
        /// <param name="func">Метод установки для каждого пикселя</param>
        public void ForEachPixelsSet(Func<TElement[]> func)
        {
            CheckReadOnly();
            try
            {
                supressUpdate = true;
                var storages = layers.Select(a => a.GetStorage(false)).ToArray();
                int len = Width * Height;
                for (int i = 0; i < len; i++)
                {
                    var result = func();
                    for (int j = 0; j < storages.Length; j++)
                        storages[j][i] = result[j];
                }
                for (int i = 0; i < storages.Length; i++)
                    layers[i].SetStorage(storages[i], false);
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].OnUpdate(Update.Full, (0, 0, Width, Height));
                }
            }
            finally
            {
                supressUpdate = false;
                OnUpdate(Update.Full, (0, 0, Width, Height));
            }
        }

        /// <summary>
        /// Установка значений для каждого пикселя на основе исходного значения
        /// </summary>
        /// <param name="func">Метод установки для каждого пикселя на основе исходного значения</param>
        public void ForEachPixelsSet(Func<TElement[], TElement[]> func)
        {
            CheckReadOnly();
            try
            {
                supressUpdate = true;
                var storages = layers.Select(a => a.GetStorage(false)).ToArray();
                int len = Width * Height;
                int layCount = layers.Length;
                for (int i = 0; i < len; i++)
                {
                    TElement[] input = new TElement[layCount];
                    for (int j = 0; j < layCount; j++)
                    {
                        input[j] = storages[j][i];
                    }
                    var result = func(input);
                    for (int j = 0; j < layCount; j++)
                    {
                        storages[j][i] = result[j];
                    }
                }
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].OnUpdate(Update.Full, (0, 0, Width, Height));
                }
            }
            finally
            {
                supressUpdate = false;
                OnUpdate(Update.Full, (0, 0, Width, Height));
            }
        }

        /// <summary>
        /// Установка значений для каждого пикселя с координатами
        /// </summary>
        /// <param name="action">Метод установки с координатами x, y для каждого пикселя</param>
        public void ForEachPixelsSet(Func<int, int, TElement[]> func)
        {
            CheckReadOnly();
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].CheckReadOnly();
            }
            try
            {
                supressUpdate = true;
                int x = 0, y = 0;
                var storages = layers.Select(a => a.GetStorage(false)).ToArray();
                int len = Width * Height;
                for (int i = 0; i < len; i++)
                {
                    if (x >= Width)
                    {
                        x = 0;
                        y++;
                    }
                    var result = func(x, y);
                    for (int j = 0; j < storages.Length; j++)
                    {
                        storages[j][i] = result[j];
                    }
                    x++;
                }
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].OnUpdate(Update.Full, (0, 0, Width, Height));
                }
            }
            finally
            {
                supressUpdate = false;
                OnUpdate(Update.Full, (0, 0, Width, Height));
            }
        }

        /// <summary>
        /// Установка значений для каждого пикселя на основе исходного значения с координатами
        /// </summary>
        /// <param name="func">Метод установки для каждого пикселя на основе исходного значения с координатами x, y</param>
        public void ForEachPixelsSet(Func<TElement[], int, int, TElement[]> func)
        {
            CheckReadOnly();
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].CheckReadOnly();
            }
            try
            {
                supressUpdate = true;
                int x = 0, y = 0;
                var storages = layers.Select(a => a.GetStorage(false)).ToArray();
                int len = Width * Height;
                int layCount = layers.Length;
                for (int i = 0; i < len; i++)
                {
                    if (x >= Width)
                    {
                        x = 0;
                        y++;
                    }
                    TElement[] input = new TElement[layCount];
                    for (int j = 0; j < layCount; j++)
                    {
                        input[j] = storages[j][i];
                    }
                    var result = func(input, x, y);
                    for (int j = 0; j < storages.Length; j++)
                    {
                        storages[j][i] = result[j];
                    }
                    x++;
                }
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].OnUpdate(Update.Full, (0, 0, Width, Height));
                }
            }
            finally
            {
                supressUpdate = false;
                OnUpdate(Update.Full, (0, 0, Width, Height));
            }
        }

        public MatrixImage<TElement> SubImage(int xstart, int ystart, int width, int height)
        {
            return new MatrixImage<TElement>(layers.Select(a => a.SubMatrix(xstart, ystart, width, height)).ToArray(), false);
        }

        public void Insert(MatrixImage<TElement> image, int xoffset, int yoffset)
        {
            CheckReadOnly();
            try
            {
                supressUpdate = true;
                var otherLayers = image.layers;
                for (int i = 0; i < layers.Length; i++)
                {
                    layers[i].Insert(otherLayers[i], xoffset, yoffset);
                }
            }
            finally
            {
                supressUpdate = false;
                OnUpdate(Update.rectangle, (xoffset, yoffset, image.Width, image.Height));
            }
        }

        public MatrixLayer<TElement> GetLayer(int indexLayer, bool copyLayer = false) =>
            copyLayer ? layers[indexLayer].Clone() : layers[indexLayer];

        private void LayerDisposed(object layer, EventArgs e)
        {
            ((MatrixLayer<TElement>)layer).Updated -= OnUpdate;
        }

        public void SetLayer(MatrixLayer<TElement> layer, int indexLayer, bool copyLayer = true)
        {
            CheckReadOnly();
            if (layer.Height != Height || layer.Width != Width) throw new Exception();
            layers[indexLayer].Disposed -= LayerDisposed;
            layers[indexLayer].Updated -= OnUpdate;
            layers[indexLayer] = copyLayer ? layer.Clone() : layer;
            layers[indexLayer].Disposed += LayerDisposed;
            layers[indexLayer].Updated += OnUpdate;
        }

        public void SetLayers(MatrixLayer<TElement>[] layers, bool copyLayer = true)
        {
            CheckReadOnly();
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = layers[i];
                if (layer.Height != Height || layer.Width != Width) throw new Exception();
                this.layers[i].Disposed -= LayerDisposed;
                this.layers[i].Updated -= OnUpdate;
                this.layers[i] = copyLayer ? layer.Clone() : layer;
                this.layers[i].Disposed += LayerDisposed;
                this.layers[i].Updated += OnUpdate;
            }
        }

        public MatrixImage<TElement> Clone() => new MatrixImage<TElement>(layers);

        object ICloneable.Clone() => Clone();

        public IEnumerator<MatrixLayer<TElement>> GetEnumerator()
        {
            foreach (var item in layers)
            {
                yield return item;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in layers)
            {
                yield return item;
            }
            yield break;
        }

        public MatrixImage<TOtherElement> ConvertTo<TOtherElement>() where TOtherElement : unmanaged, IComparable<TOtherElement>
        {
            if (ElementType == typeof(TOtherElement))
            {
                var clone = Clone();
                return Unsafe.As<MatrixImage<TElement>, MatrixImage<TOtherElement>>(ref clone);
            }
            return new MatrixImage<TOtherElement>(layers.Select(a => a.ConvertTo<TOtherElement>()).ToArray(), false);
        }

        public IMatrixImage ConvertTo(Type type)
        {
            if (ElementType == type) return Clone();
            else
            {
                var arr = layers.Select(a => a.ConvertTo(type)).ToArray();
                return MatrixImageBuilder.CreateImage(layers);
            }
        }

        public MatrixLayer<TElement> CreateSingleGray()
        {
            var ens = this.Split().Select(a => a.GetEnumerator()).ToArray();
            TElement[] storage = new TElement[this.Width * this.Height];
            int layerCount = ens.Length;
            for (int i = 0; i < storage.Length; i++)
            {
                if (typeof(TElement) == typeof(byte))
                {
                    int intsum = default;
                    foreach (var item in ens)
                    {
                        item.MoveNext();
                        var t = item.Current;
                        intsum += Unsafe.As<TElement, byte>(ref t);
                    }
                    var ret = (byte)(intsum / layerCount);
                    storage[i] = Unsafe.As<byte, TElement>(ref ret);
                }
                else
                if (typeof(TElement) == typeof(short))
                {
                    int intsum = default;
                    foreach (var item in ens)
                    {
                        item.MoveNext();
                        var t = item.Current;
                        intsum += Unsafe.As<TElement, short>(ref t);
                    }
                    var ret = (short)(intsum / layerCount);
                    storage[i] = Unsafe.As<short, TElement>(ref ret);
                }
                else
                {
                    TElement sum = default;
                    foreach (var item in ens)
                    {
                        item.MoveNext();
                        sum = MathUtil.Add(sum, item.Current);
                    }
                    storage[i] = MathUtil.DivToInt(sum, layerCount);
                }
            }
            return new MatrixLayer<TElement>(this.Width, this.Height, storage, false);
        }

        IMatrixLayer IMatrixImage.CreateSingleGray() => CreateSingleGray();

        MatrixImage<TOtherElement> IMatrixImage.ConvertTo<TOtherElement>()
        {
            return ConvertTo<TOtherElement>();
        }

        IMatrixLayer IMatrixImage.GetLayer(int indexLayer, bool copyLayer)
        {
            return GetLayer(indexLayer, copyLayer);
        }

        void IMatrixImage.Insert(IMatrixImage image, int xoffset, int yoffset)
        {
            this.Insert((MatrixImage<TElement>)image, xoffset, yoffset);
        }

        void IMatrixImage.SetLayer(IMatrixLayer layer, int indexLayer, bool copyLayer)
        {
            this.SetLayer((MatrixLayer<TElement>)layer, indexLayer, copyLayer);
        }

        IMatrixLayer[] IMatrixImage.Split(bool copyLayers)
        {
            return Split(copyLayers);
        }

        IMatrixImage IMatrixImage.SubImage(int xstart, int ystart, int width, int height)
        {
            return SubImage(xstart, ystart, width, height);
        }

        IMatrixImage IMatrixImage.ConvertTo(Type elementType)
        {
            return ConvertTo(elementType);
        }

        void IMatrixImage.ForEachPixels(Action<object[]> action)
        {
            int layerCount = LayerCount;
            var arr = new object[layerCount];
            void proxy(TElement[] vs)
            {
                Array.Copy(vs, arr, layerCount);
                action.Invoke(arr);
            }

            ForEachPixels(proxy);
        }

        void IMatrixImage.ForEachPixels(Action<object[], int, int> action)
        {
            int layerCount = LayerCount;
            var arr = new object[layerCount];
            void proxy(TElement[] vs, int x, int y)
            {
                Array.Copy(vs, arr, layerCount);
                action.Invoke(arr, x, y);
            }

            ForEachPixels(proxy);
        }

        void IMatrixImage.ForEachPixelsSet(Func<object[]> func)
        {
            int layerCount = LayerCount;
            var arr = new TElement[layerCount];
            TElement[] proxy()
            {
                var vs = func.Invoke();
                Array.Copy(vs, arr, layerCount);
                return arr;
            }

            ForEachPixelsSet(proxy);
        }

        void IMatrixImage.ForEachPixelsSet(Func<object[], object[]> func)
        {
            int layerCount = LayerCount;
            var arr1 = new object[layerCount];
            var arr2 = new TElement[layerCount];
            TElement[] proxy(TElement[] input)
            {
                Array.Copy(input, arr1, layerCount);
                var vs = func.Invoke(arr1);
                Array.Copy(arr1, arr2, layerCount);
                return arr2;
            }

            ForEachPixelsSet(proxy);
        }

        void IMatrixImage.ForEachPixelsSet(Func<int, int, object[]> func)
        {
            int layerCount = LayerCount;
            var arr = new TElement[layerCount];
            TElement[] proxy(int x, int y)
            {
                var vs = func.Invoke(x, y);
                Array.Copy(vs, arr, layerCount);
                return arr;
            }

            ForEachPixelsSet(proxy);
        }

        void IMatrixImage.ForEachPixelsSet(Func<object[], int, int, object[]> func)
        {
            int layerCount = LayerCount;
            var arr1 = new object[layerCount];
            var arr2 = new TElement[layerCount];
            TElement[] proxy(TElement[] input, int x, int y)
            {
                Array.Copy(input, arr1, layerCount);
                var vs = func.Invoke(arr1, x, y);
                Array.Copy(arr1, arr2, layerCount);
                return arr2;
            }

            ForEachPixelsSet(proxy);
        }

        IEnumerator<IMatrixLayer> IEnumerable<IMatrixLayer>.GetEnumerator()
        {
            foreach (var item in layers)
                yield return item;
        }

        #region IDisposable Support

        private bool disposedValue = false; // Для определения избыточных вызовов
        public bool IsDisposed => disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
                if (layers != null)
                    for (int i = 0; i < layers.Length; i++)
                    {
                        layers[i].Disposed -= LayerDisposed;
                        layers[i].Updated -= OnUpdate;
                        layers[i] = null;
                    }
                //MakeReadOnly(false, false);
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        ~MatrixImage()
        {
            Dispose(false);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Width), Width);
            info.AddValue(nameof(Height), Height);
            info.AddValue(nameof(layers), layers);
            info.AddValue(nameof(Tags), Tags.Where(a => a.Value.IsSerializable).ToDictionary(a => a.Key, a => a.Value));
        }

        protected MatrixImage(SerializationInfo serializationInfo, StreamingContext streamingContext) : this()
        {
            Width = serializationInfo.GetInt32(nameof(Width));
            Height = serializationInfo.GetInt32(nameof(Height));
            layers = (MatrixLayer<TElement>[])serializationInfo.GetValue(nameof(layers), typeof(MatrixLayer<TElement>[]));
            Tags = (Dictionary<string, TagInfo>)serializationInfo.GetValue(nameof(Tags), typeof(Dictionary<string, TagInfo>));
            //Tags = dict.ToDictionary(a => a.Key, a => a.Value);
        }

        protected MatrixImage()
        {
            this.sukeys = new HashSet<AutoDisposable>();
            this.rokeys = new HashSet<AutoDisposable>();
            this.disposedValue = false;
            this.supressUpdate = false;
        }

        [Reactive] public WriteableBitmap Bitmap { get; private set; }
        
        public WriteableBitmap CreateBitmap()
        {
            if (Bitmap == null)
            {
                Bitmap = new WriteableBitmap(
                    new PixelSize(Width, Height),
                    new Vector(96, 96),
                    PixelFormat.Bgra8888);
                UpdateBitmap();
            }
            return Bitmap;
        }

        private unsafe void UpdateBitmap()
        {
            if (Bitmap == null) return;
            using (ILockedFramebuffer buf = Bitmap.Lock())
            {
                uint* ptr = (uint*)buf.Address;

                var storages = ToByteImage(false).layers.Select(a => a.GetStorage(false)).ToArray();
                int len = Width * Height;

                if(layers.Length == 1)
                {
                    var storage = storages[0];
                    for (int i = 0; i < len; i++)
                        *(ptr + i) = GetColor(storage[i]);
                }
                else if (layers.Length == 3)
                {
                    var storageB = storages[0];
                    var storageG = storages[1];
                    var storageR = storages[2];
                    for (int i = 0; i < len; i++)
                        *(ptr + i) = GetColor(storageB[i], storageG[i], storageR[i]);
                }
                else if (layers.Length == 4)
                {
                    var storageB = storages[0];
                    var storageG = storages[1];
                    var storageR = storages[2];
                    var storageA = storages[3];
                    for (int i = 0; i < len; i++)
                        *(ptr + i) = GetColor(storageB[i], storageG[i], storageR[i], storageA[i]);
                }
            }
        }

        public static uint GetColor(byte b, byte g, byte r, byte a)
        {
            return (uint)(b | g << 8 | r << 16 | a << 24);
        }

        public static uint GetColor(byte b, byte g, byte r)
        {
            return (uint)(b | g << 8 | r << 16 | 0xFF000000);
        }

        public static uint GetColor(byte gray)
        {
            return (uint)(gray | gray << 8 | gray << 16 | 0xFF000000);
        }
    }
}