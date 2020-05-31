/// Автор: Лялин Максим ИС-116
/// @2020

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using OpenCvSharp;
using ImageLib.Utils;

namespace ImageLib.Image
{
    public static class MatrixLayerBuilder
    {
        public static IMatrixLayer CreateLayer(Type elementType, int width, int height)
        {
            return (IMatrixLayer)Activator.CreateInstance(typeof(MatrixLayer<>).MakeGenericType(elementType), width, height);
        }

        public static IMatrixLayer CreateLayer(Array matrix)
        {
            Type elementType = matrix.GetType().GetElementType();
            return (IMatrixLayer)Activator.CreateInstance(typeof(MatrixLayer<>).MakeGenericType(elementType), matrix);
        }

        public static IMatrixLayer CreateLayer(int width, int height, Array singleArray, bool copyArray = true)
        {
            Type elementType = singleArray.GetType().GetElementType();
            return (IMatrixLayer)Activator.CreateInstance(typeof(MatrixLayer<>).MakeGenericType(elementType), width, height, singleArray, copyArray);
        }

        public static IMatrixLayer CreateLayer(int width, int height, IEnumerable elements)
        {
            int length = width * height;
            if (width * height == 0) return new MatrixLayer<byte>(width, height);
            var en = elements.GetEnumerator();
            if (!en.MoveNext()) return new MatrixLayer<byte>(width, height);
            object first = en.Current;
            Type elementType = first.GetType();
            Array arr = Array.CreateInstance(elementType, length);
            arr.SetValue(first, 0);
            for (int i = 1; i < length && en.MoveNext(); i++)
            {
                arr.SetValue(en.Current, i);
            }
            return (IMatrixLayer)Activator.CreateInstance(typeof(MatrixLayer<>).MakeGenericType(elementType), width, height, arr, false);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class MatrixLayer<TElement> : ISerializable, IEnumerable<TElement>, ICloneable, IMatrixLayer where TElement : unmanaged/*, IComparable<TElement>*/
    {
        public Dictionary<string, TagInfo> Tags { get; }
                = new Dictionary<string, TagInfo>();

        public MatrixLayer(int width, int height, TElement defaultValue) : this()
        {
            int len = width * height;
            storage = new TElement[len];
            for (int i = 0; i < len; i++)
            {
                storage[len] = defaultValue;
            }
            Width = width;
            Height = height;
        }

        public MatrixLayer(int width, int height) : this()
        {
            storage = new TElement[width * height];
            Width = width;
            Height = height;
        }

        public MatrixLayer(int width, int height, TElement[] storage, bool copyStorage = true) : this()
        {
            Width = width;
            Height = height;
            SetStorage(storage, copyStorage);
        }

        public MatrixLayer(TElement[,] matrix) : this()
        {
            Width = matrix.GetLength(0);
            Height = matrix.GetLength(1);
            storage = new TElement[Width * Height];
            SetStorage(matrix);
        }

        public MatrixLayer(int width, int height, IEnumerable<TElement> elements) : this()
        {
            Width = width;
            Height = height;
            SetStorage(elements.Take(width * height).ToArray(), false);
        }

        public TElement this[int index]
        {
            get => storage[index];
            set
            {
                storage[index] = value;
                OnUpdate(Update.pixel, (index % Width, index / Width, 1, 1));
            }
        }

        public TElement this[int x, int y]
        {
            get => storage[GetIndex(x, y)];
            set
            {
                storage[GetIndex(x, y)] = value;
                OnUpdate(Update.pixel, (x, y, 1, 1));
            }
        }

        public static IEnumerable<(T1, T2)> ConcatToTuple<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second)
        {
            var en1 = first.GetEnumerator();
            var en2 = second.GetEnumerator();
            while (en1.MoveNext() && en2.MoveNext())
            {
                yield return (en1.Current, en2.Current);
            }
            yield break;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MatrixLayer<TOtherElement> ConvertTo<TOtherElement>(Func<TElement, TOtherElement> converter = null) where TOtherElement : unmanaged, IComparable<TOtherElement>
        {
            if (converter == null)
            {
                if (typeof(TOtherElement) == typeof(TElement))
                {
                    var clone = Clone();
                    return Unsafe.As<MatrixLayer<TElement>, MatrixLayer<TOtherElement>>(ref clone);
                }
                return new MatrixLayer<TOtherElement>(Width, Height, this.Cast<TOtherElement>());
            }
            else return new MatrixLayer<TOtherElement>(Width, Height, this.Select(converter));
        }

        public static MatrixLayer<TElement> operator +(MatrixLayer<TElement> layer, MatrixLayer<TElement> otherlayer)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height,
                ConcatToTuple(layer, otherlayer).Select(a => MathUtil.Add(a.Item1, a.Item2)));
        }

        public static MatrixLayer<TElement> operator +(MatrixLayer<TElement> layer, TElement element)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height, layer.Select(a => MathUtil.Add(a, element)));
        }

        public static MatrixLayer<TElement> operator -(MatrixLayer<TElement> layer, MatrixLayer<TElement> otherlayer)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height,
                ConcatToTuple(layer, otherlayer).Select(a => MathUtil.Sub(a.Item1, a.Item2)));
        }

        public static MatrixLayer<TElement> operator -(MatrixLayer<TElement> layer, TElement element)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height, layer.Select(a => MathUtil.Sub(a, element)));
        }

        public static MatrixLayer<TElement> operator *(MatrixLayer<TElement> layer, TElement element)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height, layer.Select(a => MathUtil.Mul(a, element)));
        }

        public static MatrixLayer<TElement> operator /(MatrixLayer<TElement> layer, TElement element)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height, layer.Select(a => MathUtil.Div(a, element)));
        }

        public static MatrixLayer<TElement> operator %(MatrixLayer<TElement> layer, TElement element)
        {
            return new MatrixLayer<TElement>(layer.Width, layer.Height, layer.Select(a => MathUtil.Mod(a, element)));
        }

        private TElement[] storage;
        public int Width { get; }
        public int Height { get; }
        public Type ElementType => typeof(TElement);

        public MatrixLayer<byte> ToByteLayer(bool cloneIfSourceByte)
        {
            if (!cloneIfSourceByte && this is MatrixLayer<byte> thisLayer) return thisLayer;
            else return ConvertTo<byte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CheckReadOnly()
        {
            if (IsReadOnly) throw new MatrixReadOnlyException();
        }

        [NonSerialized]
        private readonly HashSet<AutoDisposable> rokeys;

        [NonSerialized]
        private readonly HashSet<AutoDisposable> sukeys;

        [NonSerialized]
        private bool supressUpdate;

        protected virtual void OnUpdate(UpdateLayer update)
        {
            if (!supressUpdate && sukeys.Count == 0) Updated?.Invoke(this, update);
        }

        public virtual void OnUpdate(Update update, (int x, int y, int width, int height)? rectangleUpdate)
        {
            if (!rectangleUpdate.HasValue) rectangleUpdate = (0, 0, Width, Height);
            if (!supressUpdate && sukeys.Count == 0) Updated?.Invoke(this, new UpdateLayer(this, update, rectangleUpdate.Value));
        }

        public event EventHandler<UpdateLayer> Updated;

        public event EventHandler Disposed;

        public bool IsReadOnly => rokeys.Count > 0;

        public AutoDisposable SupressUpdating()
        {
            AutoDisposable key = new AutoDisposable() { AutoRelease = true };
            sukeys.Add(key);
            void release()
            {
                sukeys.Remove(key);
                if (sukeys.Count == 0) OnUpdate(Update.Full, null);
                key.Released -= release;
            }
            key.Released += release;
            return key;
        }

        public AutoDisposable MakeReadOnly(bool autoRelease = true)
        {
            AutoDisposable key = new AutoDisposable() { AutoRelease = autoRelease };
            rokeys.Add(key);
            void release()
            {
                rokeys.Remove(key);
                key.Released -= release;
            }
            key.Released += release;
            return key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(int x, int y) => x + y * Width;

        public void SetPoint(TElement value, int x, int y)
        {
            CheckReadOnly();
            storage[GetIndex(x, y)] = value;
            OnUpdate(Update.pixel, (x, y, 1, 1));
        }

        public object GetPoint(int x, int y)
        {
            return storage[GetIndex(x, y)];
        }

        public void SetStorage(TElement[] storage, bool copyStorage = true)
        {
            CheckReadOnly();
            if (copyStorage)
            {
                this.storage = new TElement[Width * Height];
                int len = Math.Min(storage.Length, this.storage.Length);
                Array.Copy(storage, this.storage, len);
            }
            else
            {
                if (storage.Length != Width * Height) throw new ArgumentOutOfRangeException(nameof(storage));
                this.storage = storage;
            }
            OnUpdate(Update.Full, (0, 0, Width, Height));
        }

        public void SetStorage(TElement[,] matrix)
        {
            CheckReadOnly();
            int height = Height;
            int width = Width;
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.storage[i++] = matrix[x, y];
                }
            }
            OnUpdate(Update.Full, (0, 0, Width, Height));
        }

        public TElement[] GetStorage(bool copyStorage = true) =>
            copyStorage ? (TElement[])storage.Clone() : storage;

        public TElement[,] GetMatrix()
        {
            int width = Width;
            int height = Height;
            TElement[,] matrix = new TElement[width, height];
            var en = storage.GetEnumerator();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    en.MoveNext();
                    matrix[x, y] = (TElement)en.Current;
                }
            }
            return matrix;
        }

        public TElement[] SubElements(int xstart, int ystart, int width, int height)
        {
            int xlimit = xstart + width, ylimit = ystart + height;
            int i = 0;
            TElement[] sub = new TElement[width * height];
            for (int y = ystart; y < ylimit; y++)
            {
                Array.Copy(storage, GetIndex(xstart, y), sub, i, width);
                i += width;
            }
            return sub;
        }

        public MatrixLayer<TElement> SubMatrix(int xstart, int ystart, int width, int height)
        {
            return new MatrixLayer<TElement>(width, height, SubElements(xstart, ystart, width, height), false);
        }

        public void Insert(MatrixLayer<TElement> layer, int xoffset, int yoffset)
        {
            CheckReadOnly();
            int width = layer.Width, height = layer.Height;
            int xlimit = xoffset + width, ylimit = yoffset + height;
            int i = 0;
            TElement[] sub = layer.storage;
            for (int y = yoffset; y < ylimit; y++)
            {
                Array.Copy(sub, i, storage, GetIndex(xoffset, y), width);
                i += width;
            }
            OnUpdate(Update.rectangle, (xoffset, yoffset, width, height));
        }

        public (TElement min, TElement max) MinMaxElement()
        {
            TElement min = storage[0], max = storage[0];
            ForEachPixels((TElement e) =>
            {
                //if (e.CompareTo(min) < 0) min = e;
                //else if (e.CompareTo(max) > 0) max = e;
                if (MathUtil.Lower(e, min)) min = e;
                else if (MathUtil.Greater(e, max)) max = e;
            });
            return (min, max);
        }

        public ((TElement value, int x, int y) min, (TElement value, int x, int y) max) MinMaxElementIndexed()
        {
            TElement min = storage[0], max = storage[0];
            int minx = 0, miny = 0, maxx = 0, maxy = 0;
            ForEachPixels((TElement e, int x, int y) =>
            {
                if (MathUtil.Lower(e, min))
                {
                    min = e;
                    minx = x;
                    miny = y;
                }
                else if (MathUtil.Greater(e, max))
                {
                    max = e;
                    maxx = x;
                    maxy = y;
                }
            });
            return ((min, minx, miny), (max, maxx, maxy));
        }

        public void Add(MatrixLayer<TElement> layer)
        {
            Add(layer.storage);
        }

        public void Add(TElement[] elements)
        {
            var len = storage.Length;
            if (len > elements.Length) throw new IndexOutOfRangeException();
            for (int i = 0; i < len; i++)
                storage[i] = MathUtil.Add(storage[i], elements[i]);
        }

        public void Add(TElement value)
        {
            var len = storage.Length;
            for (int i = 0; i < len; i++)
                storage[i] = MathUtil.Add(storage[i], value);
        }

        public void Sub(MatrixLayer<TElement> layer)
        {
            Sub(layer.storage);
        }

        public void Sub(TElement[] elements, bool matrixIsLeft = true)
        {
            var len = storage.Length;
            if (len > elements.Length) throw new IndexOutOfRangeException();
            if (matrixIsLeft)
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Sub(storage[i], elements[i]);
            }
            else
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Sub(elements[i], storage[i]);
            }
        }

        public void Sub(TElement value, bool matrixIsLeft = true)
        {
            var len = storage.Length;
            if (matrixIsLeft)
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Sub(storage[i], value);
            }
            else
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Sub(value, storage[i]);
            }
        }

        public void Mul(TElement value)
        {
            var len = storage.Length;
            for (int i = 0; i < len; i++)
                storage[i] = MathUtil.Mul(storage[i], value);
        }

        private void Div(TElement value, bool matrixIsLeft = true)
        {
            var len = storage.Length;
            if (matrixIsLeft)
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Div(storage[i], value);
            }
            else
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Div(value, storage[i]);
            }
        }

        public void Mod(TElement value, bool matrixIsLeft = true)
        {
            var len = storage.Length;
            if (matrixIsLeft)
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Mod(storage[i], value);
            }
            else
            {
                for (int i = 0; i < len; i++)
                    storage[i] = MathUtil.Mod(value, storage[i]);
            }
        }

        void IMatrixLayer.Add(IMatrixLayer layer) => Add((MatrixLayer<TElement>)layer);

        void IMatrixLayer.Add(object value, bool matrixIsLeft) => Add((TElement)Convert.ChangeType(value, ElementType));

        void IMatrixLayer.Add(object[] vs, bool useTypeConversion, bool matrixIsLeft)
        {
            int len = storage.Length;
            if (len > vs.Length) throw new IndexOutOfRangeException();
            if (useTypeConversion)
            {
                var elementType = ElementType;
                for (int i = 0; i < len; i++)
                {
                    storage[i] = MathUtil.Add(storage[i], (TElement)Convert.ChangeType(vs[i], elementType));
                }
            }
            else
            {
                TElement[] elements = new TElement[len];
                for (int i = 0; i < len; i++)
                {
                    elements[i] = (TElement)vs[i];
                }
                Add(elements);
            }
        }

        void IMatrixLayer.Sub(IMatrixLayer layer) => Sub((MatrixLayer<TElement>)layer);

        void IMatrixLayer.Sub(object value, bool matrixIsLeft) => Sub((TElement)Convert.ChangeType(value, ElementType), matrixIsLeft);

        void IMatrixLayer.Sub(object[] vs, bool matrixIsLeft, bool useTypeConversion)
        {
            int len = storage.Length;
            if (len > vs.Length) throw new IndexOutOfRangeException();
            if (useTypeConversion)
            {
                var elementType = ElementType;
                if (matrixIsLeft)
                    for (int i = 0; i < len; i++)
                        storage[i] = MathUtil.Sub(storage[i], (TElement)Convert.ChangeType(vs[i], elementType));
                else
                    for (int i = 0; i < len; i++)
                        storage[i] = MathUtil.Sub((TElement)Convert.ChangeType(vs[i], elementType), storage[i]);
            }
            else
            {
                TElement[] elements = new TElement[len];
                for (int i = 0; i < len; i++)
                    elements[i] = (TElement)vs[i];
                Sub(elements, matrixIsLeft);
            }
        }

        void IMatrixLayer.Mul(object value, bool matrixIsLeft) => Mul((TElement)Convert.ChangeType(value, ElementType));

        void IMatrixLayer.Div(object value, bool matrixIsLeft) => Div((TElement)Convert.ChangeType(value, ElementType), matrixIsLeft);

        void IMatrixLayer.Mod(object value, bool matrixIsLeft) => Mod((TElement)Convert.ChangeType(value, ElementType), matrixIsLeft);

        public void ForEachPixels(Action<TElement> action)
        {
            foreach (var item in storage)
            {
                action(item);
            }
        }

        public void ForEachPixels(Action<TElement, int, int> action)
        {
            int x = 0, y = 0;
            foreach (var item in storage)
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

        public void ForEachPixelsSet(Func<TElement> func)
        {
            CheckReadOnly();
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i] = func();
            }
            OnUpdate(Update.Full, (0, 0, Width, Height));
        }

        public void ForEachPixelsSet(Func<TElement, TElement> func)
        {
            CheckReadOnly();
            for (int i = 0; i < storage.Length; i++)
            {
                storage[i] = func(storage[i]);
            }
            OnUpdate(Update.Full, (0, 0, Width, Height));
        }

        public void ForEachPixelsSet(Func<int, int, TElement> func)
        {
            CheckReadOnly();
            int x = 0, y = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                if (x >= Width)
                {
                    x = 0;
                    y++;
                }
                storage[i] = func(x, y);
                x++;
            }
        }

        public void ForEachPixelsSet(Func<TElement, int, int, TElement> func)
        {
            CheckReadOnly();
            int x = 0, y = 0;
            for (int i = 0; i < storage.Length; i++)
            {
                if (x >= Width)
                {
                    x = 0;
                    y++;
                }
                storage[i] = func(storage[i], x, y);
                x++;
            }
        }

        public IEnumerable<(TElement value, int x, int y)> EnumerateIndexed()
        {
            int x = 0;
            int y = 0;
            foreach (TElement item in storage)
            {
                if (x >= Width)
                {
                    x = 0;
                    y++;
                }
                yield return (item, x, y);
                x++;
            }
            yield break;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            foreach (TElement item in storage)
            {
                yield return item;
            }
            yield break;
        }

        public IMatrixLayer ConvertTo(Type elementType)
        {
            if (this.ElementType == elementType) return Clone();
            else return MatrixLayerBuilder.CreateLayer(Width, Height, storage.Select(a => Convert.ChangeType(a, elementType)).ToArray(), false);
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TElement>)storage).GetEnumerator();

        public MatrixLayer<TElement> Clone() => new MatrixLayer<TElement>(Width, Height, storage);

        object ICloneable.Clone() => Clone();

        MatrixLayer<TOtherElement> IMatrixLayer.ConvertTo<TOtherElement>(Func<object, TOtherElement> converter)
        {
            return this.ConvertTo<TOtherElement>(null);
        }

        IEnumerable<(object value, int x, int y)> IMatrixLayer.EnumerateIndexed()
        {
            return this.EnumerateIndexed().Select(a => ((object)a.value, a.x, a.y));
        }

        int IMatrixLayer.GetIndex(int x, int y)
        {
            return this.GetIndex(x, y);
        }

        Array IMatrixLayer.GetMatrix()
        {
            return this.GetMatrix();
        }

        object IMatrixLayer.GetPoint(int x, int y)
        {
            return this.GetPoint(x, y);
        }

        Array IMatrixLayer.GetStorage(bool copyStorage)
        {
            return this.GetStorage(copyStorage);
        }

        void IMatrixLayer.Insert(IMatrixLayer layer, int xoffset, int yoffset)
        {
            this.Insert((MatrixLayer<TElement>)layer, xoffset, yoffset);
        }

        (object min, object max) IMatrixLayer.MinMaxElement()
        {
            var (min, max) = this.MinMaxElement();
            return (min, max);
        }

        ((object value, int x, int y) min, (object value, int x, int y) max) IMatrixLayer.MinMaxElementIndexed()
        {
            var (min, max) = this.MinMaxElementIndexed();
            return ((min.value, min.x, min.y), (max.value, max.x, max.y));
        }

        void IMatrixLayer.SetPoint(object value, int x, int y)
        {
            SetPoint((TElement)value, x, y);
        }

        void IMatrixLayer.SetStorage(Array storage, bool copyStorage)
        {
            if (storage.Rank == 1) SetStorage((TElement[])storage, copyStorage);
            else SetStorage((TElement[,])storage);
        }

        Array IMatrixLayer.SubElements(int xstart, int ystart, int width, int height)
        {
            return this.SubElements(xstart, ystart, width, height);
        }

        IMatrixLayer IMatrixLayer.SubMatrix(int xstart, int ystart, int width, int height)
        {
            return this.SubMatrix(xstart, ystart, width, height);
        }

        IMatrixLayer IMatrixLayer.ConvertTo(Type elementType)
        {
            return ConvertTo(elementType);
        }

        public MatrixImage<TElement> CreateImage()
        {
            return new MatrixImage<TElement>(new MatrixLayer<TElement>[] { this }, false);
        }

        IMatrixImage IMatrixLayer.CreateImage() => CreateImage();

        #region IDisposable Support

        [NonSerialized]
        private bool disposedValue; // Для определения избыточных вызовов

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
                storage = null;
                MakeReadOnly(false);
                disposedValue = true;
                Disposed?.Invoke(this, EventArgs.Empty);
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        ~MatrixLayer()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
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
            info.AddValue(nameof(storage), storage);
            info.AddValue(nameof(Tags), Tags.Where(a => a.Value.IsSerializable).ToDictionary(a => a.Key, a => a.Value));
        }

        public object GetValue(int index)
        {
            throw new NotImplementedException();
        }

        protected MatrixLayer(SerializationInfo serializationInfo, StreamingContext streamingContext) : this()
        {
            Width = (int)serializationInfo.GetValue(nameof(Width), typeof(int));
            Height = (int)serializationInfo.GetValue(nameof(Height), typeof(int));
            storage = (TElement[])serializationInfo.GetValue(nameof(storage), typeof(TElement[]));
            Tags = (Dictionary<string, TagInfo>)serializationInfo.GetValue(nameof(Tags), typeof(Dictionary<string, TagInfo>));
            //var dict = (IEnumerable<KeyValuePair<string, TagInfo>>)serializationInfo.GetValue(nameof(Tags), typeof(IEnumerable<KeyValuePair<string, TagInfo>>));
            //Tags = dict.ToDictionary(a => a.Key, a => a.Value);
        }

        protected MatrixLayer()
        {
            this.rokeys = new HashSet<AutoDisposable>();
            this.sukeys = new HashSet<AutoDisposable>();
            this.disposedValue = false;
            this.supressUpdate = false;
        }
    }
}