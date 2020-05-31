using System;
using System.Runtime.CompilerServices;

namespace ImageLib.Utils
{
    /// <summary>
    /// Предоставляет различные операции над базовыми (<see langword="unmanaged"/>) числовыми типами :
    /// <see cref="byte"/>, <see cref="sbyte"/>, <see cref="short"/>, <see cref="ushort"/>,
    /// <see cref="int"/>, <see cref="uint"/>, <see cref="float"/>, <see cref="double"/>.
    /// </summary>
    public static partial class MathUtil
    {
        // Jit оптимизатор скомпилирует это так, будто мы используем a + b, а не вызываем этот метод. Ничего лишнего
        /// <summary>
        /// Сложение
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">Левый операнд</param>
        /// <param name="right">Правый операнд</param>
        /// <returns>Результат</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Add<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = Unsafe.As<T, byte>(ref left) + Unsafe.As<T, byte>(ref right);
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = Unsafe.As<T, sbyte>(ref left) + Unsafe.As<T, sbyte>(ref right);
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = Unsafe.As<T, short>(ref left) + Unsafe.As<T, short>(ref right);
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = Unsafe.As<T, ushort>(ref left) + Unsafe.As<T, ushort>(ref right);
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = Unsafe.As<T, int>(ref left) + Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = Unsafe.As<T, uint>(ref left) + Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref sum);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = Unsafe.As<T, long>(ref left) + Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = Unsafe.As<T, ulong>(ref left) + Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = Unsafe.As<T, float>(ref left) + Unsafe.As<T, float>(ref right);
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = Unsafe.As<T, double>(ref left) + Unsafe.As<T, double>(ref right);
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Mul<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = Unsafe.As<T, byte>(ref left) * Unsafe.As<T, byte>(ref right);
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = Unsafe.As<T, sbyte>(ref left) * Unsafe.As<T, sbyte>(ref right);
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = Unsafe.As<T, short>(ref left) * Unsafe.As<T, short>(ref right);
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = Unsafe.As<T, ushort>(ref left) * Unsafe.As<T, ushort>(ref right);
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = Unsafe.As<T, int>(ref left) * Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = Unsafe.As<T, uint>(ref left) * Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref sum);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = Unsafe.As<T, long>(ref left) * Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = Unsafe.As<T, ulong>(ref left) * Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = Unsafe.As<T, float>(ref left) * Unsafe.As<T, float>(ref right);
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = Unsafe.As<T, double>(ref left) * Unsafe.As<T, double>(ref right);
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Sub<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = Unsafe.As<T, byte>(ref left) - Unsafe.As<T, byte>(ref right);
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = Unsafe.As<T, sbyte>(ref left) - Unsafe.As<T, sbyte>(ref right);
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = Unsafe.As<T, short>(ref left) - Unsafe.As<T, short>(ref right);
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = Unsafe.As<T, ushort>(ref left) - Unsafe.As<T, ushort>(ref right);
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = Unsafe.As<T, int>(ref left) - Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = Unsafe.As<T, uint>(ref left) - Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref sum);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = Unsafe.As<T, long>(ref left) - Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = Unsafe.As<T, ulong>(ref left) - Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = Unsafe.As<T, float>(ref left) - Unsafe.As<T, float>(ref right);
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = Unsafe.As<T, double>(ref left) - Unsafe.As<T, double>(ref right);
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Div<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = Unsafe.As<T, byte>(ref left) / Unsafe.As<T, byte>(ref right);
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = Unsafe.As<T, sbyte>(ref left) / Unsafe.As<T, sbyte>(ref right);
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = Unsafe.As<T, short>(ref left) / Unsafe.As<T, short>(ref right);
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = Unsafe.As<T, ushort>(ref left) / Unsafe.As<T, ushort>(ref right);
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = Unsafe.As<T, int>(ref left) / Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = Unsafe.As<T, uint>(ref left) / Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref sum);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = Unsafe.As<T, long>(ref left) / Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = Unsafe.As<T, ulong>(ref left) / Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = Unsafe.As<T, float>(ref left) / Unsafe.As<T, float>(ref right);
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = Unsafe.As<T, double>(ref left) / Unsafe.As<T, double>(ref right);
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Mod<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var sum = Unsafe.As<T, byte>(ref left) % Unsafe.As<T, byte>(ref right);
                byte ret;
                if (sum > byte.MaxValue) ret = byte.MaxValue;
                else if (sum < byte.MinValue) ret = byte.MinValue;
                else ret = (byte)sum;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var sum = Unsafe.As<T, sbyte>(ref left) % Unsafe.As<T, sbyte>(ref right);
                sbyte ret;
                if (sum > sbyte.MaxValue) ret = sbyte.MaxValue;
                else if (sum < sbyte.MinValue) ret = sbyte.MinValue;
                else ret = (sbyte)sum;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var sum = Unsafe.As<T, short>(ref left) % Unsafe.As<T, short>(ref right);
                short ret;
                if (sum > short.MaxValue) ret = short.MaxValue;
                else if (sum < short.MinValue) ret = short.MinValue;
                else ret = (short)sum;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var sum = Unsafe.As<T, ushort>(ref left) % Unsafe.As<T, ushort>(ref right);
                ushort ret;
                if (sum > ushort.MaxValue) ret = ushort.MaxValue;
                else if (sum < ushort.MinValue) ret = ushort.MinValue;
                else ret = (ushort)sum;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var sum = Unsafe.As<T, int>(ref left) % Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref sum);
            }
            else if (typeof(T) == typeof(uint))
            {
                var sum = Unsafe.As<T, uint>(ref left) % Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref sum);
            }
            else if (typeof(T) == typeof(long))
            {
                var sum = Unsafe.As<T, long>(ref left) % Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref sum);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var sum = Unsafe.As<T, ulong>(ref left) % Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref sum);
            }
            else if (typeof(T) == typeof(float))
            {
                var sum = Unsafe.As<T, float>(ref left) % Unsafe.As<T, float>(ref right);
                return Unsafe.As<float, T>(ref sum);
            }
            else if (typeof(T) == typeof(double))
            {
                var sum = Unsafe.As<T, double>(ref left) % Unsafe.As<T, double>(ref right);
                return Unsafe.As<double, T>(ref sum);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Lower<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                return Unsafe.As<T, byte>(ref left) < Unsafe.As<T, byte>(ref right);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return Unsafe.As<T, sbyte>(ref left) < Unsafe.As<T, sbyte>(ref right);
            }
            else if (typeof(T) == typeof(short))
            {
                return Unsafe.As<T, short>(ref left) < Unsafe.As<T, short>(ref right);
            }
            else if (typeof(T) == typeof(ushort))
            {
                return Unsafe.As<T, ushort>(ref left) < Unsafe.As<T, ushort>(ref right);
            }
            else if (typeof(T) == typeof(int))
            {
                return Unsafe.As<T, int>(ref left) < Unsafe.As<T, int>(ref right);
            }
            else if (typeof(T) == typeof(uint))
            {
                return Unsafe.As<T, uint>(ref left) < Unsafe.As<T, uint>(ref right);
            }
            else if (typeof(T) == typeof(long))
            {
                return Unsafe.As<T, long>(ref left) < Unsafe.As<T, long>(ref right);
            }
            else if (typeof(T) == typeof(ulong))
            {
                return Unsafe.As<T, ulong>(ref left) < Unsafe.As<T, ulong>(ref right);
            }
            else if (typeof(T) == typeof(float))
            {
                return Unsafe.As<T, float>(ref left) < Unsafe.As<T, float>(ref right);
            }
            else if (typeof(T) == typeof(double))
            {
                return Unsafe.As<T, double>(ref left) < Unsafe.As<T, double>(ref right);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Greater<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                return Unsafe.As<T, byte>(ref left) > Unsafe.As<T, byte>(ref right);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return Unsafe.As<T, sbyte>(ref left) > Unsafe.As<T, sbyte>(ref right);
            }
            else if (typeof(T) == typeof(short))
            {
                return Unsafe.As<T, short>(ref left) > Unsafe.As<T, short>(ref right);
            }
            else if (typeof(T) == typeof(ushort))
            {
                return Unsafe.As<T, ushort>(ref left) > Unsafe.As<T, ushort>(ref right);
            }
            else if (typeof(T) == typeof(int))
            {
                return Unsafe.As<T, int>(ref left) > Unsafe.As<T, int>(ref right);
            }
            else if (typeof(T) == typeof(uint))
            {
                return Unsafe.As<T, uint>(ref left) > Unsafe.As<T, uint>(ref right);
            }
            else if (typeof(T) == typeof(long))
            {
                return Unsafe.As<T, long>(ref left) > Unsafe.As<T, long>(ref right);
            }
            else if (typeof(T) == typeof(ulong))
            {
                return Unsafe.As<T, ulong>(ref left) > Unsafe.As<T, ulong>(ref right);
            }
            else if (typeof(T) == typeof(float))
            {
                return Unsafe.As<T, float>(ref left) > Unsafe.As<T, float>(ref right);
            }
            else if (typeof(T) == typeof(double))
            {
                return Unsafe.As<T, double>(ref left) > Unsafe.As<T, double>(ref right);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LowerOrEquals<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                return Unsafe.As<T, byte>(ref left) <= Unsafe.As<T, byte>(ref right);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return Unsafe.As<T, sbyte>(ref left) <= Unsafe.As<T, sbyte>(ref right);
            }
            else if (typeof(T) == typeof(short))
            {
                return Unsafe.As<T, short>(ref left) <= Unsafe.As<T, short>(ref right);
            }
            else if (typeof(T) == typeof(ushort))
            {
                return Unsafe.As<T, ushort>(ref left) <= Unsafe.As<T, ushort>(ref right);
            }
            else if (typeof(T) == typeof(int))
            {
                return Unsafe.As<T, int>(ref left) <= Unsafe.As<T, int>(ref right);
            }
            else if (typeof(T) == typeof(uint))
            {
                return Unsafe.As<T, uint>(ref left) <= Unsafe.As<T, uint>(ref right);
            }
            else if (typeof(T) == typeof(long))
            {
                return Unsafe.As<T, long>(ref left) <= Unsafe.As<T, long>(ref right);
            }
            else if (typeof(T) == typeof(ulong))
            {
                return Unsafe.As<T, ulong>(ref left) <= Unsafe.As<T, ulong>(ref right);
            }
            else if (typeof(T) == typeof(float))
            {
                return Unsafe.As<T, float>(ref left) <= Unsafe.As<T, float>(ref right);
            }
            else if (typeof(T) == typeof(double))
            {
                return Unsafe.As<T, double>(ref left) <= Unsafe.As<T, double>(ref right);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GreaterOrEquals<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                return Unsafe.As<T, byte>(ref left) >= Unsafe.As<T, byte>(ref right);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return Unsafe.As<T, sbyte>(ref left) >= Unsafe.As<T, sbyte>(ref right);
            }
            else if (typeof(T) == typeof(short))
            {
                return Unsafe.As<T, short>(ref left) >= Unsafe.As<T, short>(ref right);
            }
            else if (typeof(T) == typeof(ushort))
            {
                return Unsafe.As<T, ushort>(ref left) >= Unsafe.As<T, ushort>(ref right);
            }
            else if (typeof(T) == typeof(int))
            {
                return Unsafe.As<T, int>(ref left) >= Unsafe.As<T, int>(ref right);
            }
            else if (typeof(T) == typeof(uint))
            {
                return Unsafe.As<T, uint>(ref left) >= Unsafe.As<T, uint>(ref right);
            }
            else if (typeof(T) == typeof(long))
            {
                return Unsafe.As<T, long>(ref left) >= Unsafe.As<T, long>(ref right);
            }
            else if (typeof(T) == typeof(ulong))
            {
                return Unsafe.As<T, ulong>(ref left) >= Unsafe.As<T, ulong>(ref right);
            }
            else if (typeof(T) == typeof(float))
            {
                return Unsafe.As<T, float>(ref left) >= Unsafe.As<T, float>(ref right);
            }
            else if (typeof(T) == typeof(double))
            {
                return Unsafe.As<T, double>(ref left) >= Unsafe.As<T, double>(ref right);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                return Unsafe.As<T, byte>(ref left).Equals(Unsafe.As<T, byte>(ref right));
            }
            else if (typeof(T) == typeof(sbyte))
            {
                return Unsafe.As<T, sbyte>(ref left).Equals(Unsafe.As<T, sbyte>(ref right));
            }
            else if (typeof(T) == typeof(short))
            {
                return Unsafe.As<T, short>(ref left).Equals(Unsafe.As<T, short>(ref right));
            }
            else if (typeof(T) == typeof(ushort))
            {
                return Unsafe.As<T, ushort>(ref left).Equals(Unsafe.As<T, ushort>(ref right));
            }
            else if (typeof(T) == typeof(int))
            {
                return Unsafe.As<T, int>(ref left).Equals(Unsafe.As<T, int>(ref right));
            }
            else if (typeof(T) == typeof(uint))
            {
                return Unsafe.As<T, uint>(ref left).Equals(Unsafe.As<T, uint>(ref right));
            }
            else if (typeof(T) == typeof(long))
            {
                return Unsafe.As<T, long>(ref left).Equals(Unsafe.As<T, long>(ref right));
            }
            else if (typeof(T) == typeof(ulong))
            {
                return Unsafe.As<T, ulong>(ref left).Equals(Unsafe.As<T, ulong>(ref right));
            }
            else if (typeof(T) == typeof(float))
            {
                return Unsafe.As<T, float>(ref left).Equals(Unsafe.As<T, float>(ref right));
            }
            else if (typeof(T) == typeof(double))
            {
                return Unsafe.As<T, double>(ref left).Equals(Unsafe.As<T, double>(ref right));
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MaxValue<T>()
        {
            if (typeof(T) == typeof(byte))
            {
                var ret = byte.MaxValue;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var ret = sbyte.MaxValue;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var ret = short.MaxValue;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var ret = ushort.MaxValue;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var ret = int.MaxValue;
                return Unsafe.As<int, T>(ref ret);
            }
            else if (typeof(T) == typeof(uint))
            {
                var ret = uint.MaxValue;
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                var ret = long.MaxValue;
                return Unsafe.As<long, T>(ref ret);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var ret = ulong.MaxValue;
                return Unsafe.As<ulong, T>(ref ret);
            }
            else if (typeof(T) == typeof(float))
            {
                var ret = float.MaxValue;
                return Unsafe.As<float, T>(ref ret);
            }
            else if (typeof(T) == typeof(double))
            {
                var ret = double.MaxValue;
                return Unsafe.As<double, T>(ref ret);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T MinValue<T>()
        {
            if (typeof(T) == typeof(byte))
            {
                var ret = byte.MinValue;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var ret = sbyte.MinValue;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var ret = short.MinValue;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var ret = ushort.MinValue;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var ret = int.MinValue;
                return Unsafe.As<int, T>(ref ret);
            }
            else if (typeof(T) == typeof(uint))
            {
                var ret = uint.MinValue;
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                var ret = long.MinValue;
                return Unsafe.As<long, T>(ref ret);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var ret = ulong.MinValue;
                return Unsafe.As<ulong, T>(ref ret);
            }
            else if (typeof(T) == typeof(float))
            {
                var ret = float.MinValue;
                return Unsafe.As<float, T>(ref ret);
            }
            else if (typeof(T) == typeof(double))
            {
                var ret = double.MinValue;
                return Unsafe.As<double, T>(ref ret);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(T left, T right) => Greater(left, right) ? left : right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(T left, T right) => Greater(left, right) ? right : left;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Zero<T>() => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T One<T>()
        {
            if (typeof(T) == typeof(byte))
            {
                byte ret = 1;
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                sbyte ret = 1;
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                short ret = 1;
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                ushort ret = 1;
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                int ret = 1;
                return Unsafe.As<int, T>(ref ret);
            }
            else if (typeof(T) == typeof(uint))
            {
                uint ret = 1;
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                long ret = 1;
                return Unsafe.As<long, T>(ref ret);
            }
            else if (typeof(T) == typeof(ulong))
            {
                ulong ret = 1;
                return Unsafe.As<ulong, T>(ref ret);
            }
            else if (typeof(T) == typeof(float))
            {
                float ret = 1.0f;
                return Unsafe.As<float, T>(ref ret);
            }
            else if (typeof(T) == typeof(double))
            {
                double ret = 1.0d;
                return Unsafe.As<double, T>(ref ret);
            }
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Negative<T>(T value) => Sub(default, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T And<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var ret = Unsafe.As<T, byte>(ref left);
                ret &= Unsafe.As<T, byte>(ref right);
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var ret = Unsafe.As<T, sbyte>(ref left);
                ret &= Unsafe.As<T, sbyte>(ref right);
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var ret = Unsafe.As<T, short>(ref left);
                ret &= Unsafe.As<T, short>(ref right);
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var ret = Unsafe.As<T, ushort>(ref left);
                ret &= Unsafe.As<T, ushort>(ref right);
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var ret = Unsafe.As<T, int>(ref left);
                ret &= Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref ret);
            }
            else if (typeof(T) == typeof(uint))
            {
                var ret = Unsafe.As<T, uint>(ref left);
                ret &= Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                var ret = Unsafe.As<T, long>(ref left);
                ret &= Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref ret);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var ret = Unsafe.As<T, ulong>(ref left);
                ret &= Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref ret);
            }
            //else if (typeof(T) == typeof(float))
            //{
            //    var ret = Unsafe.As<T, float>(ref left);
            //    ret &= Unsafe.As<T, float>(ref right);
            //    return Unsafe.As<float, T>(ref ret);
            //}
            //else if (typeof(T) == typeof(double))
            //{
            //    var sum = Unsafe.As<T, double>(ref left) + Unsafe.As<T, double>(ref right);
            //    return Unsafe.As<double, T>(ref sum);
            //}
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Or<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var ret = Unsafe.As<T, byte>(ref left);
                ret |= Unsafe.As<T, byte>(ref right);
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var ret = Unsafe.As<T, sbyte>(ref left);
                ret |= Unsafe.As<T, sbyte>(ref right);
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var ret = Unsafe.As<T, short>(ref left);
                ret |= Unsafe.As<T, short>(ref right);
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var ret = Unsafe.As<T, ushort>(ref left);
                ret |= Unsafe.As<T, ushort>(ref right);
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var ret = Unsafe.As<T, int>(ref left);
                ret |= Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref ret);
            }
            else if (typeof(T) == typeof(uint))
            {
                var ret = Unsafe.As<T, uint>(ref left);
                ret |= Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                var ret = Unsafe.As<T, long>(ref left);
                ret |= Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref ret);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var ret = Unsafe.As<T, ulong>(ref left);
                ret |= Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref ret);
            }
            //else if (typeof(T) == typeof(float))
            //{
            //    var ret = Unsafe.As<T, float>(ref left);
            //    ret &= Unsafe.As<T, float>(ref right);
            //    return Unsafe.As<float, T>(ref ret);
            //}
            //else if (typeof(T) == typeof(double))
            //{
            //    var sum = Unsafe.As<T, double>(ref left) + Unsafe.As<T, double>(ref right);
            //    return Unsafe.As<double, T>(ref sum);
            //}
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Xor<T>(T left, T right)
        {
            if (typeof(T) == typeof(byte))
            {
                var ret = Unsafe.As<T, byte>(ref left);
                ret ^= Unsafe.As<T, byte>(ref right);
                return Unsafe.As<byte, T>(ref ret);
            }
            else if (typeof(T) == typeof(sbyte))
            {
                var ret = Unsafe.As<T, sbyte>(ref left);
                ret ^= Unsafe.As<T, sbyte>(ref right);
                return Unsafe.As<sbyte, T>(ref ret);
            }
            else if (typeof(T) == typeof(short))
            {
                var ret = Unsafe.As<T, short>(ref left);
                ret ^= Unsafe.As<T, short>(ref right);
                return Unsafe.As<short, T>(ref ret);
            }
            else if (typeof(T) == typeof(ushort))
            {
                var ret = Unsafe.As<T, ushort>(ref left);
                ret ^= Unsafe.As<T, ushort>(ref right);
                return Unsafe.As<ushort, T>(ref ret);
            }
            else if (typeof(T) == typeof(int))
            {
                var ret = Unsafe.As<T, int>(ref left);
                ret ^= Unsafe.As<T, int>(ref right);
                return Unsafe.As<int, T>(ref ret);
            }
            else if (typeof(T) == typeof(uint))
            {
                var ret = Unsafe.As<T, uint>(ref left);
                ret ^= Unsafe.As<T, uint>(ref right);
                return Unsafe.As<uint, T>(ref ret);
            }
            else if (typeof(T) == typeof(long))
            {
                var ret = Unsafe.As<T, long>(ref left);
                ret ^= Unsafe.As<T, long>(ref right);
                return Unsafe.As<long, T>(ref ret);
            }
            else if (typeof(T) == typeof(ulong))
            {
                var ret = Unsafe.As<T, ulong>(ref left);
                ret ^= Unsafe.As<T, ulong>(ref right);
                return Unsafe.As<ulong, T>(ref ret);
            }
            //else if (typeof(T) == typeof(float))
            //{
            //    var ret = Unsafe.As<T, float>(ref left);
            //    ret &= Unsafe.As<T, float>(ref right);
            //    return Unsafe.As<float, T>(ref ret);
            //}
            //else if (typeof(T) == typeof(double))
            //{
            //    var sum = Unsafe.As<T, double>(ref left) + Unsafe.As<T, double>(ref right);
            //    return Unsafe.As<double, T>(ref sum);
            //}
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanBeNegative<T>()
        {
            if (typeof(T) == typeof(byte))
                return false;
            else if (typeof(T) == typeof(sbyte))
                return true;
            else if (typeof(T) == typeof(short))
                return true;
            else if (typeof(T) == typeof(ushort))
                return false;
            else if (typeof(T) == typeof(int))
                return true;
            else if (typeof(T) == typeof(uint))
                return false;
            else if (typeof(T) == typeof(long))
                return true;
            else if (typeof(T) == typeof(ulong))
                return false;
            else if (typeof(T) == typeof(float))
                return true;
            else if (typeof(T) == typeof(double))
                return true;
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanBeNegative(Type type)
        {
            if (type == typeof(byte))
                return false;
            else if (type == typeof(sbyte))
                return true;
            else if (type == typeof(short))
                return true;
            else if (type == typeof(ushort))
                return false;
            else if (type == typeof(int))
                return true;
            else if (type == typeof(uint))
                return false;
            else if (type == typeof(long))
                return true;
            else if (type == typeof(ulong))
                return false;
            else if (type == typeof(float))
                return true;
            else if (type == typeof(double))
                return true;
            throw new NotSupportedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPositive<T>(T value)
        {
            return Greater(value, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNegative<T>(T value)
        {
            return CanBeNegative<T>() && Lower(value, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefault<T>(T value)
        {
            return Equals(value, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOne<T>(T value)
        {
            return Equals(value, One<T>());
        }
    }
}