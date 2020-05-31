using System;
using System.Collections.Generic;

namespace ImageLib.Loader
{
    [Obsolete("Не реализовано как следует", true)]
    public interface IImageTypeInfo
    {
        Type ImageType { get; }
        Type LayerType { get; }
        Type InterfaceMethod { get; }
        object CloneImage(object image);
        object CloneLayer(object layer);
        object ConvertElementTypeImage(object image, Type type);
        object ConvertElementTypeLayer(object layer, Type type);
        object ConvertToOtherImage(object image, Type type);
        Array[] CopyToArrayImage(object image);
        Array CopyToArrayLayer(object layer);
        object CreateFromArrayImage(Array array);
        object CreateFromArrayImage(Array[] array);
        object CreateFromArrayLayer(Array array);
        object CreateImage(Type elementType, int imageWidth, int imageHeight, int layerCount);
        object CreateLayer(Type elementType, int layerWidth, int layerHeight);
        IEnumerable<Array> EnumerateElementsFromImage(object image);
        IEnumerable<(Array element, int x, int y)> EnumerateElementsFromImageIndexed(object image);
        IEnumerable<object> EnumerateElementsFromLayer(object layer);
        IEnumerable<(object element, int x, int y)> EnumerateElementsFromLayerIndexed(object layer);
        void ForEachElementImage(object image, Action<Array> action);
        void ForEachElementImageIndexed(object image, Action<Array, int, int> action);
        void ForEachElementLayer(object layer, Action<object> action);
        void ForEachElementLayerIndexed(object layer, Action<object, int, int> action);
        Array GetElementPointFromImage(object image, int x, int y);
        object GetElementPointFromLayer(object layer, int x, int y);
        Type GetElementTypeFromImage(object image);
        Type GetElementTypeFromLayer(object layer);
        object GetLayer(object image, int layerIndex);
        (int width, int height) GetSizeImage(object image);
        (int width, int height) GetSizeLayer(object layer);
        object Merge(Array layers);
        void SetElementPointToImage(object image, Array element, int x, int y);
        void SetElementPointToLayer(object layer, object element, int x, int y);
        void SetForEachElementImage(object image, Func<Array> func);
        void SetForEachElementImageIndexed(object image, Func<int, int, Array> func);
        void SetForEachElementLayer(object layer, Func<object> func);
        void SetForEachElementLayerIndexed(object layer, Func<int, int, object> func);
        Array Split(object image);
    }
}