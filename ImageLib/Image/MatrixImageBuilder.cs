/// Автор: Лялин Максим ИС-116
/// @2020

using System;

namespace ImageLib.Image
{
    public static class MatrixImageBuilder
    {
        public static IMatrixImage CreateImage(Type elementType, int width, int height, int layersCount)
        {
            return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(elementType), width, height, layersCount);
        }

        public static IMatrixImage CreateImage(Type elementType, int width, int height, int layersCount, object defaultValue)
        {
            return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(elementType), width, height, layersCount, defaultValue);
        }

        public static IMatrixImage CreateImage(IMatrixLayer[] layers, bool copyLayers)
        {
            Type elementType = layers[0].ElementType;
            var array = Array.CreateInstance(typeof(MatrixLayer<>).MakeGenericType(elementType), layers.Length);
            for (int i = 0; i < layers.Length; i++)
            {
                array.SetValue(layers[i], i);
            }
            return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(elementType), array, copyLayers);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="layers">
        /// В зависимости от типа <see cref="Array"/>
        /// <list type="bullet">
        /// <item><term>Двумерный массив простых чисел T[,]</term> вернет однослойное изображение</item>
        /// <item><term>Массив двумерных массивов простых чисел T[][,]</term> вернет многослойное изображение</item>
        /// <item><term>Массив слоев IMatrixLayer[]</term> вернет изображение, состоящее из данных слоев без копирования</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public static IMatrixImage CreateImage(Array layers)
        {
            var elementType = layers.GetType().GetElementType();
            if (elementType.IsValueType)
            {
                if (layers.Rank == 2)
                {
                    // layers is ?[,]
                    IMatrixLayer layer = MatrixLayerBuilder.CreateLayer(layers);
                    layers = Array.CreateInstance(typeof(MatrixLayer<>).MakeGenericType(layer.ElementType), 1);
                    layers.SetValue(layer, 0);
                    return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(layer.ElementType), layers, false);
                }
            }
            else if (elementType.IsArray)
            {
                elementType = elementType.GetElementType();
                if (elementType.GetArrayRank() == 2 && (typeof(IMatrixLayer).IsAssignableFrom(elementType)))
                {
                    Array[] subarrays = (Array[])layers;
                    layers = Array.CreateInstance(elementType.GetElementType(), subarrays.Length);
                    for (int i = 0; i < subarrays.Length; i++)
                    {
                        layers.SetValue(MatrixLayerBuilder.CreateLayer(layers), i);
                    }
                    return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(elementType), layers);
                }
            }
            if (typeof(IMatrixLayer) == elementType)
            {
                return CreateImage((IMatrixLayer[])layers, false);
            }
            if (typeof(IMatrixLayer).IsAssignableFrom(elementType))
            {
                // layers is MatrixLayer<?>[]
                return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(elementType.GenericTypeArguments[0]), layers, false);
            }
            return (IMatrixImage)Activator.CreateInstance(typeof(MatrixImage<>).MakeGenericType(elementType), layers);
        }
    }
}