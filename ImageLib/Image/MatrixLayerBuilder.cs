/// Автор: Лялин Максим ИС-116
/// @2020

using System;
using System.Collections;

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
}