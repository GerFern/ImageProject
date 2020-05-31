using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageLib.Utils
{
    public static partial class MathUtil
    {
        public static IEnumerable<T> Add<T>(this IEnumerable<T> vs, T value) =>
            vs.Select(a => Add(a, value));

        public static IEnumerable<T> Sub<T>(this IEnumerable<T> vs, T value, bool enumIsLeft = true) =>
            enumIsLeft ?
                vs.Select(a => Sub(a, value)) :
                vs.Select(a => Sub(value, a));

        public static IEnumerable<T> Mul<T>(this IEnumerable<T> vs, T value) =>
               vs.Select(a => Mul(a, value));

        public static IEnumerable<T> Div<T>(this IEnumerable<T> vs, T value, bool enumIsLeft = true) =>
           enumIsLeft ?
               vs.Select(a => Div(a, value)) :
               vs.Select(a => Div(value, a));

        public static IEnumerable<T> Mod<T>(this IEnumerable<T> vs, T value, bool enumIsLeft = true) =>
          enumIsLeft ?
              vs.Select(a => Mod(a, value)) :
              vs.Select(a => Mod(value, a));

        public static void ForEachAdd<T>(this T[] vs, T value)
        {
            var length = vs.Length;
            for (int i = 0; i < length; i++)
                vs[i] = Add(vs[i], value);
        }

        public static void ForEachSub<T>(this T[] vs, T value, bool arrayIsLeft = true)
        {
            var length = vs.Length;
            if (arrayIsLeft)
                for (int i = 0; i < length; i++)
                    vs[i] = Sub(vs[i], value);
            else
                for (int i = 0; i < length; i++)
                    vs[i] = Sub(value, vs[i]);
        }

        public static void ForEachMul<T>(this T[] vs, T value)
        {
            var length = vs.Length;
            for (int i = 0; i < length; i++)
                vs[i] = Mul(vs[i], value);
        }

        public static void ForEachDiv<T>(this T[] vs, T value, bool arrayIsLeft = true)
        {
            var length = vs.Length;
            if (arrayIsLeft)
                for (int i = 0; i < length; i++)
                    vs[i] = Div(vs[i], value);
            else
                for (int i = 0; i < length; i++)
                    vs[i] = Div(value, vs[i]);
        }

        public static void ForEachMod<T>(this T[] vs, T value, bool arrayIsLeft = true)
        {
            var length = vs.Length;
            if (arrayIsLeft)
                for (int i = 0; i < length; i++)
                    vs[i] = Mod(vs[i], value);
            else
                for (int i = 0; i < length; i++)
                    vs[i] = Mod(value, vs[i]);
        }

        public static void QuickSort<T>(this T[] vs, int start, int end)
        {
            static int partition(T[] array, int start, int end)
            {
                T temp;//swap helper
                int marker = start;//divides left and right subarrays
                for (int i = start; i < end; i++)
                {
                    if (Lower(array[i], array[end])) //array[end] is pivot
                    {
                        temp = array[marker]; // swap
                        array[marker] = array[i];
                        array[i] = temp;
                        marker += 1;
                    }
                }
                //put pivot(array[end]) between left and right subarrays
                temp = array[marker];
                array[marker] = array[end];
                array[end] = temp;
                return marker;
            }

            if (start >= end) return;
            int pivot = partition(vs, start, end);
            QuickSort(vs, start, pivot - 1);
            QuickSort(vs, pivot + 1, end);
        }
    }
}