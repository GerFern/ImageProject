using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Linq;
using System.ComponentModel;

namespace ImageLib.Loader
{
    [Obsolete]
    public class FormatRegister
    {
    }

    //public class ImageTypeInfo
    //{

    //    public ImageTypeInfo(Type imageType, Type layerType)
    //    {
    //        ImageType = imageType;
    //        LayerType = layerType;
    //    }

    //    public Type ImageType { get; }
    //    public Type LayerType { get; }

    //    //public Func<int, object> GetLayer { get; set; }
    //    ///// <summary>
    //    ///// <list type="bullet">
    //    ///// <item><term>arg1</term> ElementType</item>
    //    ///// <item><term>arg2</term> LayerSize</item>
    //    ///// <item><term>ret</term> Layer</item>
    //    ///// </list>
    //    ///// </summary>
    //    //public Func<Type, Size, object> CreateLayer { get; set; }
    //    ///// <summary>
    //    ///// <list type="bullet">
    //    ///// <item><term>arg1</term> ElementType</item>
    //    ///// <item><term>arg2</term> ImageSize</item>
    //    ///// <item><term>arg3</term> LayerCounts</item>
    //    ///// </list>
    //    ///// </summary>
    //    //public Func<Type, Size, int, object> CreateImage { get; set; }
    //    ///// <summary>
    //    ///// 
    //    ///// </summary>
    //    //public Func<object[], object> CreateImageFromLayers { get; set; }

    //}

    [Obsolete]
    public abstract class ImageTypeInfo<TImage, TLayer, TInterface> : IImageTypeInfo
    {
        private static Dictionary<Type, IImageTypeInfo> otherTypesLayer = new Dictionary<Type, IImageTypeInfo>();
        private static Dictionary<Type, IImageTypeInfo> otherTypesImage = new Dictionary<Type, IImageTypeInfo>();

        public Type ImageType => typeof(TImage);
        public Type LayerType => typeof(TLayer);
        public Type InterfaceMethod => typeof(TInterface);

        public abstract TLayer GetLayer(TImage image, int layerIndex);
        public abstract TLayer CreateLayer(Type elementType, int layerWidth, int layerHeight);
        public virtual TLayer CreateLayer<T>(int layerWidth, int layerHeight) =>
            CreateLayer(typeof(T), layerWidth, layerHeight);
        public abstract TImage CreateImage(Type elementType, int imageWidth, int imageHeight, int layerCount);
        public virtual TImage CreateImage<T>(int imageWidth, int imageHeight, int layerCount) =>
            CreateImage(typeof(T), imageWidth, imageHeight, layerCount);

        public abstract TLayer[] Split(TImage image);
        public abstract TImage Merge(TLayer[] layers);
        public abstract Type GetElementTypeFromImage(TImage image);
        public abstract Type GetElementTypeFromLayer(TLayer layer);


        public abstract (int width, int height) GetSizeLayer(TLayer layer);
        public abstract (int width, int height) GetSizeImage(TImage image);
        public abstract int GetLayerCount(TImage image);

        public abstract object GetElementPointFromLayer(TLayer layer, int x, int y);
        public virtual T GetElementPointFromLayer<T>(TLayer layer, int x, int y) =>
            (T)GetElementPointFromLayer(layer, x, y);
        public abstract Array GetElementPointFromImage(TImage image, int x, int y);
        public virtual T[] GetElementPointFromImage<T>(TImage image, int x, int y) =>
            (T[])GetElementPointFromImage(image, x, y);

        public abstract void SetElementPointToLayer(TLayer layer, object element, int x, int y);
        public virtual void SetElementPointToLayer<T>(TLayer layer, T element, int x, int y) =>
            SetElementPointToLayer(layer, element, x, y);
        public abstract void SetElementPointToImage(TImage image, Array element, int x, int y);
        public virtual void SetElementPointToImage<T>(TImage image, T[] element, int x, int y) =>
            SetElementPointToImage(image, element, x, y);


        public virtual IEnumerable<object> EnumerateElementsFromLayer(TLayer layer)
        {
            var (width, height) = GetSizeLayer(layer);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return GetElementPointFromLayer(layer, x, y);
                }
            }
        }
        public virtual IEnumerable<(object element, int x, int y)> EnumerateElementsFromLayerIndexed(TLayer layer)
        {
            var (width, height) = GetSizeLayer(layer);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return (GetElementPointFromLayer(layer, x, y), x, y);
                }
            }
        }
        public virtual IEnumerable<T> EnumerateElementsFromLayer<T>(TLayer layer)
        {
            var (width, height) = GetSizeLayer(layer);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return GetElementPointFromLayer<T>(layer, x, y);
                }
            }
        }
        public virtual IEnumerable<(T element, int x, int y)> EnumerateElementsFromLayerIndexed<T>(TLayer layer)
        {
            var (width, height) = GetSizeLayer(layer);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return (GetElementPointFromLayer<T>(layer, x, y), x, y);
                }
            }
        }

        public virtual IEnumerable<Array> EnumerateElementsFromImage(TImage image)
        {
            var (width, height) = GetSizeImage(image);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return GetElementPointFromImage(image, x, y);
                }
            }
        }
        public virtual IEnumerable<(Array element, int x, int y)> EnumerateElementsFromImageIndexed(TImage image)
        {
            var (width, height) = GetSizeImage(image);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yield return (GetElementPointFromImage(image, x, y), x, y);
                }
            }
        }
        public virtual IEnumerable<T[]> EnumerateElementsFromImage<T>(TImage image)
        {
            return EnumerateElementsFromImage(image).Select(a => a.Cast<T>().ToArray());
        }
        public virtual IEnumerable<(T[] element, int x, int y)> EnumerateElementsFromImageIndexed<T>(TImage image)
        {
            return EnumerateElementsFromImageIndexed(image).Select(a => (a.element.Cast<T>().ToArray(), a.x, a.y));
        }



        public virtual void ForEachElementLayer(TLayer layer, Action<object> action)
        {
            foreach (var item in EnumerateElementsFromLayer(layer))
                action(item);
        }
        public virtual void ForEachElementLayerIndexed(TLayer layer, Action<object, int, int> action)
        {
            foreach (var (element, x, y) in EnumerateElementsFromLayerIndexed(layer))
                action(element, x, y);
        }
        public virtual void ForEachElementLayer<T>(TLayer layer, Action<T> action)
        {
            ForEachElementLayer(layer, el => action((T)el));
        }
        public virtual void ForEachElementLayerIndexed<T>(TLayer layer, Action<T, int, int> action)
        {
            ForEachElementLayerIndexed(layer, (el, x, y) => action((T)el, x, y));
        }

        public virtual void ForEachElementImage(TImage image, Action<Array> action)
        {
            foreach (var item in EnumerateElementsFromImage(image))
                action(item);
        }
        public virtual void ForEachElementImageIndexed(TImage image, Action<Array, int, int> action)
        {
            foreach (var (element, x, y) in EnumerateElementsFromImageIndexed(image))
                action(element, x, y);
        }
        public virtual void ForEachElementImage<T>(TImage image, Action<T[]> action)
        {
            ForEachElementImage(image, el => action(el.Cast<T>().ToArray()));

        }
        public virtual void ForEachElementImageIndexed<T>(TImage image, Action<T[], int, int> action)
        {
            ForEachElementImageIndexed(image, (el, x, y) => action(el.Cast<T>().ToArray(), x, y));
        }

        public virtual void SetForEachElementLayer(TLayer layer, Func<object> func)
        {
            var (width, height) = GetSizeLayer(layer);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.SetElementPointToLayer(layer, func(), x, y);
                }
            }
        }
        public virtual void SetForEachElementLayerIndexed(TLayer layer, Func<int, int, object> func)
        {
            var (width, height) = GetSizeLayer(layer);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.SetElementPointToLayer(layer, func(x, y), x, y);
                }
            }
        }

        public virtual void SetForEachElementImage(TImage image, Func<Array> func)
        {
            var (width, height) = GetSizeImage(image);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.SetElementPointToImage(image, func(), x, y);
                }
            }
        }
        public virtual void SetForEachElementImageIndexed(TImage image, Func<int, int, Array> func)
        {
            var (width, height) = GetSizeImage(image);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    this.SetElementPointToImage(image, func(x, y), x, y);
                }
            }
        }
        public virtual void SetForEachElementLayer<T>(TLayer layer, Func<T> func) =>
            SetForEachElementLayer(layer, () => func());
        public virtual void SetForEachElementLayerIndexed<T>(TLayer layer, Func<int, int, T> func)
        {
            SetForEachElementLayerIndexed(layer, (x, y) => func(x, y));
        }

        public virtual void SetForEachElementImage<T>(TImage image, Func<T[]> func) =>
            SetForEachElementImage(image, () => func());
        public virtual void SetForEachElementImageIndexed<T>(TImage image, Func<int, int, T[]> func)
        {
            SetForEachElementImageIndexed(image, (x, y) => func(x, y));
        }

        private IEnumerable<object> EnumerateTwoRankArray(Array array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return array.GetValue(x, y);
                }
            }
        }

        private IEnumerable<T> EnumerateTwoRankArray<T>(T[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return array[x, y];
                }
            }
        }

        private IEnumerable<object> EnumerateThreeRankArray(Array array, bool layerFix)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int layers = array.GetLength(2);

            if (layerFix)
            {
                for (int l = 0; l < layers; l++)
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                            yield return array.GetValue(x, y, l);
            }
            else
            {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        for (int l = 0; l < layers; l++)
                            yield return array.GetValue(x, y, l);
            }
        }

        private IEnumerable<T> EnumerateThreeRankArray<T>(T[,,] array, bool layerFix)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int layers = array.GetLength(2);

            if (layerFix)
            {
                for (int l = 0; l < layers; l++)
                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                            yield return array[x, y, l];
            }
            else
            {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        for (int l = 0; l < layers; l++)
                            yield return array[x, y, l];
            }
        }

        public virtual TLayer CreateFromArrayLayer(Array array)
        {
            if (array.Rank == 2)
            {
                TLayer layer = CreateLayer(array.GetType().GetElementType(), array.GetLength(0), array.GetLength(1));
                var en = EnumerateTwoRankArray(array).GetEnumerator();

                SetForEachElementLayer(layer, () =>
                {
                    en.MoveNext();
                    return en.Current;
                });
                return layer;
            }
            else throw new Exception();
        }

        public virtual TLayer CreateFromArrayLayer<T>(T[,] array)
        {
            return CreateFromArrayLayer((Array)array);
        }

        public virtual TImage CreateFromArrayImage(Array array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            int layersCount = array.GetLength(2);
            Type elementType = array.GetType().GetElementType();

            if (array.Rank == 3)
            {
                Array[] arrays = new Array[layersCount];
                TLayer[] layers = new TLayer[layersCount];
                var en = EnumerateThreeRankArray(array, true).GetEnumerator();

                for (int i = 0; i < layersCount; i++)
                {
                    Array arr;
                    arrays[i] = arr = Array.CreateInstance(elementType, width, height);
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            en.MoveNext();
                            arr.SetValue(en.Current, x, y);
                        }
                    }
                    layers[i] = CreateFromArrayLayer(arr);
                }
                return Merge(layers);
            }
            else throw new Exception();
        }

        public virtual TImage CreateFromArrayImage<T>(T[,,] array)
        {
            return CreateFromArrayImage((Array)array);
        }

        public virtual TImage CreateFromArrayImage(Array[] array) =>
            Merge(array.Select(a => CreateFromArrayLayer(a)).ToArray());

        public virtual TImage CreateFromArrayImage<T>(T[,][] array) =>
            CreateFromArrayImage(array as Array[]);

        public virtual Array CopyToArrayLayer(TLayer layer)
        {
            var (width, height) = GetSizeLayer(layer);
            var mat = Array.CreateInstance(GetElementTypeFromLayer(layer), width, height);
            var e = EnumerateElementsFromLayer(layer).GetEnumerator();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    e.MoveNext();
                    mat.SetValue(e.Current, x, y);
                }
            }
            return mat;
        }

        public virtual T[,] CopyToArrayLayer<T>(TLayer layer)
        {
            return (T[,])CopyToArrayLayer(layer);
            //var arr = CopyToArrayLayer(layer);
            //int width = arr.GetLength(0);
            //int height = arr.GetLength(1);
            //var mat = new T[width, height];
            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        mat[x, y] = (T)arr.GetValue(x, y);
            //    }
            //}
            //return mat;
        }

        public virtual Array[] CopyToArrayImage(TImage image)
        {
            var (width, height) = GetSizeImage(image);
            Type elementType = GetElementTypeFromImage(image);
            TLayer[] layers = Split(image);

            var mats = (Array[])Array.CreateInstance(elementType.MakeArrayType(2), layers.Length);
            //var mat = new object[width, height][];

            for (int i = 0; i < layers.Length; i++)
            {
                mats.SetValue(CopyToArrayLayer(layers[i]), i);
            }

            return mats;
        }

        public virtual T[,][] CopyToArrayImage<T>(TImage image)
        {
            return CopyToArrayImage(image) as T[,][];
            //var arr = CopyToArrayImage(image);
            //int width = arr.GetLength(0);
            //int height = arr.GetLength(1);
            //var mat = new T[width, height][];
            //for (int y = 0; y < height; y++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        mat[x, y] = arr[x, y].Cast<T>().ToArray();
            //    }
            //}
            //return mat;
        }

        public virtual TLayer CloneLayer(TLayer layer)
        {
            var (width, height) = GetSizeLayer(layer);
            TLayer clone = CreateLayer(GetElementTypeFromLayer(layer), width, height);
            var en = EnumerateElementsFromLayer(layer).GetEnumerator();
            SetForEachElementLayer(layer, () =>
            {
                en.MoveNext();
                return en.Current;
            });
            return clone;
        }

        public virtual TImage CloneImage(TImage image)
        {
            TLayer[] split = Split(image);
            TLayer[] clone = split.Select(a => CloneLayer(a)).ToArray();
            return Merge(clone);
        }


        Dictionary<Type, Func<TLayer, object>> layerConverters = new Dictionary<Type, Func<TLayer, object>>();
        Dictionary<Type, Func<TImage, object>> imageConverters = new Dictionary<Type, Func<TImage, object>>();

        public object ConvertToOtherLayer(TLayer layer, Type type)
        {
            if (layerConverters.TryGetValue(type, out Func<TLayer, object> converter))
            {
                return converter(layer);
            }
            else if (layer.GetType() == type) return CloneLayer(layer);
            else
            {
                var format = otherTypesLayer[type];
                var formatType = format.GetType();
                var arr = CopyToArrayLayer(layer);
                return format.CreateFromArrayLayer(arr);
            }
        }

        public object ConvertToOtherImage(TImage image, Type type)
        {
            if (imageConverters.TryGetValue(type, out Func<TImage, object> converter))
            {
                return converter(image);
            }
            else if (image.GetType() == type) return CloneImage(image);
            else
            {
                var format = otherTypesImage[type];
                var formatType = format.GetType();
                var arr = CopyToArrayImage(image);
                return format.CreateFromArrayImage(arr);
            }
        }

        public virtual TLayer ConvertElementTypeLayer(TLayer layer, Type type)
        {
            if (GetElementTypeFromLayer(layer).Equals(type))
            {
                return CloneLayer(layer);
            }
            else
            {

                var (width, height) = GetSizeLayer(layer);
                TLayer clone = CreateLayer(GetElementTypeFromLayer(layer), width, height);
                var en = EnumerateElementsFromLayer(layer).GetEnumerator();
                SetForEachElementLayer(layer, () =>
                {
                    en.MoveNext();
                    return Convert.ChangeType(en.Current, type);
                });
                return clone;
            }
        }

        public virtual TLayer ConvertElementTypeLayer<TElement>(TLayer layer)
        {
            return ConvertElementTypeLayer(layer, typeof(TElement));
        }

        public virtual TImage ConvertElementTypeImage(TImage image, Type type)
        {
            if (GetElementTypeFromImage(image).Equals(type))
            {
                return CloneImage(image);
            }
            else
            {
                TLayer[] split = Split(image);
                TLayer[] clone = split.Select(a => ConvertElementTypeLayer(a, type)).ToArray();
                return Merge(clone);
            }
        }

        public virtual TImage ConvertElementTypeImage<TElement>(TImage image)
        {
            return ConvertElementTypeImage(image, typeof(TElement));
        }

        object IImageTypeInfo.CloneImage(object image)
        {
            return this.CloneImage((TImage)image);
        }

        object IImageTypeInfo.CloneLayer(object layer)
        {
            return this.CloneLayer((TLayer)layer);
        }

        object IImageTypeInfo.ConvertElementTypeImage(object image, Type type)
        {
            return this.ConvertElementTypeImage((TImage)image, type);
        }

        object IImageTypeInfo.ConvertElementTypeLayer(object layer, Type type)
        {
            return this.ConvertElementTypeLayer((TLayer)layer, type);
        }

        object IImageTypeInfo.ConvertToOtherImage(object image, Type type)
        {
            return this.ConvertToOtherImage((TImage)image, type);
        }

        Array[] IImageTypeInfo.CopyToArrayImage(object image)
        {
            return this.CopyToArrayImage((TImage)image);
        }

        Array IImageTypeInfo.CopyToArrayLayer(object layer)
        {
            return this.CopyToArrayLayer((TLayer)layer);
        }

        object IImageTypeInfo.CreateFromArrayImage(Array array)
        {
            return this.CreateFromArrayImage(array);
        }

        object IImageTypeInfo.CreateFromArrayImage(Array[] array)
        {
            return this.CreateFromArrayImage(array);
        }

        object IImageTypeInfo.CreateFromArrayLayer(Array array)
        {
            return this.CreateFromArrayLayer(array);
        }

        object IImageTypeInfo.CreateImage(Type elementType, int imageWidth, int imageHeight, int layerCount)
        {
            return this.CreateImage(elementType, imageWidth, imageHeight, layerCount);
        }

        object IImageTypeInfo.CreateLayer(Type elementType, int layerWidth, int layerHeight)
        {
            return this.CreateLayer(elementType, layerWidth, layerHeight);
        }

        IEnumerable<Array> IImageTypeInfo.EnumerateElementsFromImage(object image)
        {
            return this.EnumerateElementsFromImage((TImage)image);
        }

        IEnumerable<(Array element, int x, int y)> IImageTypeInfo.EnumerateElementsFromImageIndexed(object image)
        {
            return this.EnumerateElementsFromImageIndexed((TImage)image);
        }

        IEnumerable<object> IImageTypeInfo.EnumerateElementsFromLayer(object layer)
        {
            return this.EnumerateElementsFromLayer((TLayer)layer);
        }

        IEnumerable<(object element, int x, int y)> IImageTypeInfo.EnumerateElementsFromLayerIndexed(object layer)
        {
            return this.EnumerateElementsFromLayerIndexed((TLayer)layer);
        }

        void IImageTypeInfo.ForEachElementImage(object image, Action<Array> action)
        {
            this.ForEachElementImage((TImage)image, action);
        }

        void IImageTypeInfo.ForEachElementImageIndexed(object image, Action<Array, int, int> action)
        {
            this.ForEachElementImageIndexed((TImage)image, action);
        }

        void IImageTypeInfo.ForEachElementLayer(object layer, Action<object> action)
        {
            this.ForEachElementLayer((TLayer)layer, action);
        }

        void IImageTypeInfo.ForEachElementLayerIndexed(object layer, Action<object, int, int> action)
        {
            this.ForEachElementLayerIndexed((TLayer)layer, action);
        }

        Array IImageTypeInfo.GetElementPointFromImage(object image, int x, int y)
        {
            return this.GetElementPointFromImage((TImage)image, x, y);
        }

        object IImageTypeInfo.GetElementPointFromLayer(object layer, int x, int y)
        {
            return this.GetElementPointFromLayer((TLayer)layer, x, y);
        }

        Type IImageTypeInfo.GetElementTypeFromImage(object image)
        {
            return this.GetElementTypeFromImage((TImage)image);
        }

        Type IImageTypeInfo.GetElementTypeFromLayer(object layer)
        {
            return GetElementTypeFromLayer((TLayer)layer);
        }

        object IImageTypeInfo.GetLayer(object image, int layerIndex)
        {
            return GetLayer((TImage)image, layerIndex);
        }

        (int width, int height) IImageTypeInfo.GetSizeImage(object image)
        {
            return this.GetSizeImage((TImage)image);
        }

        (int width, int height) IImageTypeInfo.GetSizeLayer(object layer)
        {
            return this.GetSizeLayer((TLayer)layer);
        }

        object IImageTypeInfo.Merge(Array layers)
        {
            return this.Merge(layers.Cast<TLayer>().ToArray());
        }

        void IImageTypeInfo.SetElementPointToImage(object image, Array element, int x, int y)
        {
            this.SetElementPointToImage((TImage)image, element, x, y);
        }

        void IImageTypeInfo.SetElementPointToLayer(object layer, object element, int x, int y)
        {
            this.SetElementPointToLayer((TLayer)layer, element, x, y);
        }

        void IImageTypeInfo.SetForEachElementImage(object image, Func<Array> func)
        {
            this.SetForEachElementImage((TImage)image, func);
        }

        void IImageTypeInfo.SetForEachElementImageIndexed(object image, Func<int, int, Array> func)
        {
            this.SetForEachElementImageIndexed((TImage)image, func);
        }

        void IImageTypeInfo.SetForEachElementLayer(object layer, Func<object> func)
        {
            this.SetForEachElementLayer((TLayer)layer, func);
        }

        void IImageTypeInfo.SetForEachElementLayerIndexed(object layer, Func<int, int, object> func)
        {
            this.SetForEachElementLayerIndexed((TLayer)layer, func);
        }

        Array IImageTypeInfo.Split(object image)
        {
            return this.Split((TImage)image);
        }
    }
}
