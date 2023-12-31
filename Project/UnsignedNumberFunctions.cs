﻿using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ouroboros.Maths;

public static class UnsignedNumberFunctions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T IsGreaterThan<T>(this T value, T other) where T : IBinaryInteger<T> =>
        (value > other).As<T>();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T Maximum<T>(this T value, T other) where T : IBinaryInteger<T> =>
        (value ^ ((value ^ other) & (-other.IsGreaterThan(other: value))));

    public static TResult ElegantPair<TInput, TResult>(this TInput value, TInput other) where TInput : IBinaryInteger<TInput>, IUnsignedNumber<TInput> where TResult : IBinaryInteger<TResult>, IUnsignedNumber<TResult> {
        var x = value.Maximum(other: other);
        var y = ((value ^ other) * (x & TInput.One));
        var z = TResult.CreateTruncating(value: x);

        return ((z * (z + TResult.One)) + (TResult.CreateTruncating(value: (y ^ other)) - TResult.CreateTruncating(value: (y ^ value))));
    }
    public static (TResult x, TResult y) ElegantUnpair<TInput, TResult>(this TInput value) where TInput : IBinaryInteger<TInput>, IUnsignedNumber<TInput> where TResult : IBinaryInteger<TResult>, IUnsignedNumber<TResult> {
        var x = value.SquareRoot();
        var y = TResult.CreateTruncating(value: (value - (x * x)));
        var z = TResult.CreateTruncating(value: x);

        if (y < z) {
            (y, z) = (z, y);
        }
        else {
            y = ((z << 1) - y);
        }

        if (TResult.IsOddInteger(value: TResult.Max(x: y, y: z))) {
            (y, z) = (z, y);
        }

        return (y, z);
    }
    public static IEnumerable<T> EnumeratePrimeFactors<T>(this T value) where T : IBinaryInteger<T>, IUnsignedNumber<T> {
        if (T.CreateChecked(value: 4) > value) { yield break; }
        if (T.CreateChecked(value: 5) == value) { yield break; }
        if (T.CreateChecked(value: 7) == value) { yield break; }
        if (T.CreateChecked(value: 11) == value) { yield break; }
        if (T.CreateChecked(value: 13) == value) { yield break; }

        var index = value;

        while (T.Zero == (index & T.One)/* enumerate factors of 2 */) {
            yield return T.CreateChecked(value: 2);

            index >>= 1;
        }
        while (T.Zero == (index % T.CreateChecked(value: 3))/* enumerate factors of 3 */) {
            yield return T.CreateChecked(value: 3);

            index /= T.CreateChecked(value: 3);
        }
        while (T.Zero == (index % T.CreateChecked(value: 5))/* enumerate factors of 5 */) {
            yield return T.CreateChecked(value: 5);

            index /= T.CreateChecked(value: 5);
        }
        while (T.Zero == (index % T.CreateChecked(value: 7))/* enumerate factors of 7 */) {
            yield return T.CreateChecked(value: 7);

            index /= T.CreateChecked(value: 7);
        }
        while (T.Zero == (index % T.CreateChecked(value: 11))/* enumerate factors of 11 */) {
            yield return T.CreateChecked(value: 11);

            index /= T.CreateChecked(value: 11);
        }
        while (T.Zero == (index % T.CreateChecked(value: 13))/* enumerate factors of 13 */) {
            yield return T.CreateChecked(value: 13);

            index /= T.CreateChecked(value: 13);
        }

        var factor = T.CreateChecked(value: 17);
        var limit = index.SquareRoot();

        if (factor <= limit) {
            do {
                while (T.Zero == (index % factor)/* enumerate factors of (30k - 13) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 2);

                while (T.Zero == (index % factor)/* enumerate factors of (30k - 11) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 4);

                while (T.Zero == (index % factor)/* enumerate factors of (30k - 7) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 6);

                while (T.Zero == (index % factor)/* enumerate factors of (30k - 1) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 2);

                while (T.Zero == (index % factor)/* enumerate factors of (30k + 1) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 6);

                while (T.Zero == (index % factor)/* enumerate factors of (30k + 7) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 4);

                while (T.Zero == (index % factor)/* enumerate factors of (30k + 11) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 2);

                while (T.Zero == (index % factor)/* enumerate factors of (30k + 13) */) {
                    yield return factor;

                    index /= factor;
                }

                factor += T.CreateChecked(value: 4);
                limit = index.SquareRoot();
            } while (factor <= limit);
        }

        if ((index != T.One) && (index != value)) {
            yield return index;
        }
    }
    /// <remarks>
    /// Based on the paper:
    ///     An Improved Integer Multiplicative Inverse(modulo 2w)
    ///     Jeffrey Hurchalla, April 2022
    ///     https://arxiv.org/ftp/arxiv/papers/2204/2204.04342.pdf
    /// </remarks>
    public static T ModularInverse<T>(this T value) where T : IBinaryInteger<T>, IUnsignedNumber<T> {
        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);
        var x = ((T.CreateChecked(value: 3) * value) ^ T.CreateChecked(value: 2));
        var y = (T.One - (value * x));

        x *= (y + T.One);

        if (bitCount > 8) {
            y *= y;
            x *= (y + T.One);
        }

        if (bitCount > 16) {
            y *= y;
            x *= (y + T.One);
        }

        if (bitCount > 32) {
            y *= y;
            x *= (y + T.One);
        }

        if (bitCount > 64) {
            y *= y;
            x *= (y + T.One);
        }

        if (bitCount > 128) {
            var i = (int.Log2(value: (bitCount / 4)) - 5);

            do {
                y *= y;
                x *= (y + T.One);
            } while (0 < --i);
        }

        return x;
    }
    public static T NextPowerOfTwo<T>(this T value) where T : IBinaryInteger<T>, IUnsignedNumber<T> {
        var x = int.CreateTruncating(value: (BinaryIntegerConstants<T>.Size - T.LeadingZeroCount(value: (value - T.One))));
        var y = int.CreateTruncating(value: BinaryIntegerConstants<T>.Log2Size);

        return ((T.One ^ T.CreateTruncating(value: (((uint)x) >> y))) << x);
    }
    public static T NextSquare<T>(this T value) where T : IBinaryInteger<T>, IUnsignedNumber<T> {
        var squareRootPlusOne = (value.SquareRoot() + T.One);

        return (squareRootPlusOne * squareRootPlusOne);
    }
    /// <remarks>
    /// Based on the paper:
    ///     Square Rooting Algorithms for Integer and Floating-Point Numbers
    ///     IEEE Transactions on Computers, August 1990
    ///     Volume: 39, Issue: 8, Pages: 1025 - 1029
    /// </remarks>
    public static T SquareRoot<T>(this T value) where T : IBinaryInteger<T>, IUnsignedNumber<T> {
        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);

        return bitCount switch {
#if !FORCE_SOFTWARE_SQRT
            8 => T.CreateTruncating(value: ((uint)MathF.Sqrt(x: uint.CreateTruncating(value: value)))),
            16 => T.CreateTruncating(value: ((uint)MathF.Sqrt(x: uint.CreateTruncating(value: value)))),
            32 => T.CreateTruncating(value: ((uint)Math.Sqrt(d: uint.CreateTruncating(value: value)))),
            64 => T.CreateTruncating(value: Sqrt(value: ulong.CreateTruncating(value: value))),
#endif
            _ => SoftwareImplementation(value: value),
        };

        /*
             Credit goes to njuffa for providing a reference implementation:
                 https://stackoverflow.com/a/31149161/1186165
             Notes:
                 - This implementation of the algorithm runs in constant time, based on the size of T.
                 - Ignoring the loop that is entered when the size of T exceeds 64, all branches get eliminated during JIT compilation.
         */
        static T SoftwareImplementation(T value) {
            var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);
            var msb = int.CreateTruncating(value: value.MostSignificantBit());
            var msbIsOdd = (msb & 1);
            var m = ((msb + 1) >> 1);
            var mMinusOne = (m - 1);
            var mPlusOne = (m + 1);
            var x = (T.One << mMinusOne);
            var y = (x - (value >> (mPlusOne - msbIsOdd)));
            var z = y;

            x += x;

            if (bitCount > 8) {
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
            }

            if (bitCount > 16) {
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
            }

            if (bitCount > 32) {
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
                y = (((y * y) >> mPlusOne) + z);
            }

            if (bitCount > 64) {
                var i = T.CreateTruncating(value: (BinaryIntegerConstants<T>.Size >> 3));

                do {
                    i -= (T.One << 3);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                    y = (((y * y) >> mPlusOne) + z);
                } while (T.Zero < i);
            }

            y = (x - y);
            x = T.CreateTruncating(value: msbIsOdd);
            y -= bitCount switch {
                8 => (x * ((y * T.CreateChecked(value: 5UL)) >> 4)),
                16 => (x * ((y * T.CreateChecked(value: 75UL)) >> 8)),
                32 => (x * ((y * T.CreateChecked(value: 19195UL)) >> 16)),
                64 => (x * ((y * T.CreateChecked(value: 1257966796UL)) >> 32)),
                128 => (x * ((y * T.CreateChecked(value: 5402926248376769403UL)) >> 64)),
                _ => throw new NotSupportedException(), // TODO: Research a way to calculate the proper constant at runtime.
            };
            x = (T.One << (bitCount - 1));
            y -= (value - (y * y)).IsGreaterThan(other: x);

            if (bitCount > 8) {
                y -= (value - (y * y)).IsGreaterThan(other: x);
                y -= (value - (y * y)).IsGreaterThan(other: x);
            }

            if (bitCount > 32) {
                y -= (value - (y * y)).IsGreaterThan(other: x);
                y -= (value - (y * y)).IsGreaterThan(other: x);
                y -= (value - (y * y)).IsGreaterThan(other: x);
            }

            return (y & (T.AllBitsSet >> 1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Sqrt(ulong value) {
            var x = ((uint)Math.Sqrt(d: unchecked((long)value)));
            var y = (unchecked(((ulong)x) * x) > value).As<uint>(); // ((x * x) > value) ? 1 : 0
            var z = ((uint)(value >> 63)); // (64 == value.MostSignificantBit()) ? 1 : 0

            return unchecked(x - (y | z));
        }
    }
}
