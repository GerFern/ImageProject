using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ImageLib.Utils
{
    public static partial class MathUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DivToInt<T>(T left, int right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = Unsafe.As<T, byte>(ref left) / right;
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = Unsafe.As<T, sbyte>(ref left) / right;
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = Unsafe.As<T, short>(ref left) / right;
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = Unsafe.As<T, ushort>(ref left) / right;
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = Unsafe.As<T, int>(ref left) / right;
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = (uint)(Unsafe.As<T, uint>(ref left) / right);
                return Unsafe.As<uint, T>(ref sum);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = Unsafe.As<T, long>(ref left) / right;
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = Unsafe.As<T, ulong>(ref left) / (ulong)right;
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = Unsafe.As<T, float>(ref left) / right;
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = Unsafe.As<T, double>(ref left) / right;
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T DivToInt<T>(int left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = left / Unsafe.As<T, byte>(ref right);
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = left / Unsafe.As<T, sbyte>(ref right);
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = left / Unsafe.As<T, short>(ref right);
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = left / Unsafe.As<T, byte>(ref right);
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = left / Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = left / Unsafe.As<T, uint>(ref right);
                uint ret;
                if (sum > ushort.MaxValue) ret = uint.MaxValue;
                else if (sum < ushort.MinValue) ret = uint.MinValue;
                else ret = (uint)sum;
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = left / Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = (ulong)left / Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = left / Unsafe.As<T, float>(ref right);
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = left / Unsafe.As<T, double>(ref right);
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }
    }
}