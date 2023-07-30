﻿using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ouroboros.Maths;

public static class BinaryIntegerFunctions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T As<T>(this bool value) where T : IBinaryInteger<T> =>
        T.CreateTruncating(value: Unsafe.As<bool, byte>(source: ref value));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T IsNonZero<T>(this T value) where T : IBinaryInteger<T> =>
        (T.Zero != value).As<T>();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T NthFermatMask<T>(this int value) where T : IBinaryInteger<T> =>
        (T.AllBitsSet / value.NthFermatNumber<T>());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T NthFermatNumber<T>(this int value) where T : IBinaryInteger<T> =>
        ((T.One << (1 << value)) + T.One);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T SetLeastSignificantBits<T>(this T value) where T : IBinaryInteger<T> =>
        (value | (value - T.One));

    public static TResult BitwisePair<TInput, TResult>(this TInput value, TInput other) where TInput : IBinaryInteger<TInput> where TResult : IBinaryInteger<TResult> {
        var x = TResult.CreateTruncating(value: value);
        var y = TResult.CreateTruncating(value: other);

        if (BinaryIntegerConstants<TResult>.Size > 128) {
            var i = (int.Log2(value: BinaryIntegerConstants<TResult>.Size) - 7);

            do {
                x = ((x | (x << (1 << i))) & i.NthFermatMask<TResult>());
                y = ((y | (y << (1 << i))) & i.NthFermatMask<TResult>());
            } while (0 < --i);
        }

        if (BinaryIntegerConstants<TResult>.Size > 64) {
            x = ((x | (x << 64)) & 6.NthFermatMask<TResult>());
            y = ((y | (y << 64)) & 6.NthFermatMask<TResult>());
        }

        if (BinaryIntegerConstants<TResult>.Size > 32) {
            x = ((x | (x << 32)) & 5.NthFermatMask<TResult>());
            y = ((y | (y << 32)) & 5.NthFermatMask<TResult>());
        }

        if (BinaryIntegerConstants<TResult>.Size > 16) {
            x = ((x | (x << 16)) & 4.NthFermatMask<TResult>());
            y = ((y | (y << 16)) & 4.NthFermatMask<TResult>());
        }

        if (BinaryIntegerConstants<TResult>.Size > 8) {
            x = ((x | (x << 8)) & 3.NthFermatMask<TResult>());
            y = ((y | (y << 8)) & 3.NthFermatMask<TResult>());
        }

        if (BinaryIntegerConstants<TResult>.Size > 4) {
            x = ((x | (x << 4)) & 2.NthFermatMask<TResult>());
            y = ((y | (y << 4)) & 2.NthFermatMask<TResult>());
        }

        if (BinaryIntegerConstants<TResult>.Size > 2) {
            x = ((x | (x << 2)) & 1.NthFermatMask<TResult>());
            y = ((y | (y << 2)) & 1.NthFermatMask<TResult>());
        }

        x = ((x | (x << 1)) & 0.NthFermatMask<TResult>());
        y = ((y | (y << 1)) & 0.NthFermatMask<TResult>());

        return (x | (y << 1));
    }
    public static T ClearLowestSetBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value & (value - T.One));
    public static T DigitalRoot<T>(this T value) where T : IBinaryInteger<T> {
        var x = value.IsNonZero();
        var y = T.Abs(value: value);
        var z = T.CreateChecked(value: 9U);

        return (x + ((y - x) % z));
    }
    public static IEnumerable<T> EnumerateDigits<T>(this T value) where T : IBinaryInteger<T> {
        var quotient = value;

        do {
            (quotient, var remainder) = T.DivRem(left: quotient, right: T.CreateTruncating(value: 10U));

            yield return remainder;
        } while (T.Zero < quotient);
    }
    public static T Exponentiate<T>(this T value, T exponent) where T : IBinaryInteger<T> {
        var result = T.One;

        do {
            if (T.One == (exponent & T.One)) {
                result *= value;
            }

            exponent >>= 1;
            value *= value;
        } while (T.Zero < exponent);

        return result;
    }
    public static T ExtractLowestSetBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value & (-value));
    public static T GreatestCommonDivisor<T>(this T value, T other) where T : IBinaryInteger<T> {
        if (T.Zero == other) { return value; }
        else if (T.Zero == value) { return other; }

        other = T.Abs(value: other);
        value = T.Abs(value: value);

        var shift = int.CreateTruncating(value: T.TrailingZeroCount(value: (other | value)));

        other >>= int.CreateTruncating(value: T.TrailingZeroCount(value: other));
        value >>= int.CreateTruncating(value: T.TrailingZeroCount(value: value));

        if (other != value) {
            do {
                var swap = ((other ^ value) & (-(value < other).As<T>()));

                other ^= swap;
                value ^= swap;
                value -= other;
                value >>= int.CreateTruncating(value: T.TrailingZeroCount(value: value));
            } while (other != value);
        }

        return (other << shift);
    }
    public static T LeastCommonMultiple<T>(this T value, T other) where T : IBinaryInteger<T> =>
        ((value / value.GreatestCommonDivisor(other: other)) * other);
    public static T LeastSignificantBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value.IsNonZero() * (T.TrailingZeroCount(value: value) + T.One));
    public static T LeastSignificantDigit<T>(this T value) where T : IBinaryInteger<T> =>
        (value % T.CreateTruncating(value: 10U));
    public static T MostSignificantBit<T>(this T value) where T : IBinaryInteger<T> =>
        (T.CreateTruncating(value: BinaryIntegerConstants<T>.Size) - T.LeadingZeroCount(value: value));
    public static T MostSignificantDigit<T>(this T value) where T : IBinaryInteger<T> =>
        (value / T.CreateTruncating(value: 10U).Exponentiate(exponent: (value.LogarithmBase10() - T.One)));
    public static T NthSquare<T>(this T value) where T : IBinaryInteger<T> =>
        (value * value);
    public static T PermuteBitsLexicographically<T>(this T value) where T : IBinaryInteger<T> {
        var x = value.SetLeastSignificantBits();
        var y = int.CreateTruncating(value: (T.TrailingZeroCount(value: value) + T.One));
        var z = (((~x).ExtractLowestSetBit() - T.One) >> y);

        return ((x + T.One) | z);
    }
    public static T PopulationParity<T>(this T value) where T : IBinaryInteger<T> =>
        (T.PopCount(value: value) & T.One);
    public static T ReflectedBinaryDecode<T>(this T value) where T : IBinaryInteger<T> {
        value ^= (value >> 1);

        if (BinaryIntegerConstants<T>.Size > 2) {
            value ^= (value >> 2);
        }

        if (BinaryIntegerConstants<T>.Size > 4) {
            value ^= (value >> 4);
        }

        if (BinaryIntegerConstants<T>.Size > 8) {
            value ^= (value >> 8);
        }

        if (BinaryIntegerConstants<T>.Size > 16) {
            value ^= (value >> 16);
        }

        if (BinaryIntegerConstants<T>.Size > 32) {
            value ^= (value >> 32);
        }

        if (BinaryIntegerConstants<T>.Size > 64) {
            value ^= (value >> 64);
        }

        if (BinaryIntegerConstants<T>.Size > 128) {
            var i = (BitOperations.Log2(value: ((uint)(BinaryIntegerConstants<T>.Size))) - 7);

            do {
                value ^= (value >> (int.CreateTruncating(value: BinaryIntegerConstants<T>.Size) >> i));
            } while (0 < --i);
        }

        return value;
    }
    public static T ReflectedBinaryEncode<T>(this T value) where T : IBinaryInteger<T> =>
        (value ^ (value >> 1));
    public static T ReverseBits<T>(this T value) where T : IBinaryInteger<T> {
        if (BinaryIntegerConstants<T>.Size > 2) {
            value = (((value >> 1) & 0.NthFermatMask<T>()) | ((value & 0.NthFermatMask<T>()) << 1));
        }

        if (BinaryIntegerConstants<T>.Size > 4) {
            value = (((value >> 2) & 1.NthFermatMask<T>()) | ((value & 1.NthFermatMask<T>()) << 2));
        }

        if (BinaryIntegerConstants<T>.Size > 8) {
            value = (((value >> 4) & 2.NthFermatMask<T>()) | ((value & 2.NthFermatMask<T>()) << 4));
        }

        if (BinaryIntegerConstants<T>.Size > 16) {
            value = (((value >> 8) & 3.NthFermatMask<T>()) | ((value & 3.NthFermatMask<T>()) << 8));
        }

        if (BinaryIntegerConstants<T>.Size > 32) {
            value = (((value >> 16) & 4.NthFermatMask<T>()) | ((value & 4.NthFermatMask<T>()) << 16));
        }

        if (BinaryIntegerConstants<T>.Size > 64) {
            value = (((value >> 32) & 5.NthFermatMask<T>()) | ((value & 5.NthFermatMask<T>()) << 32));
        }

        if (BinaryIntegerConstants<T>.Size > 128) {
            var index = 0;
            var limit = (int.Log2(value: BinaryIntegerConstants<T>.Size) - 7);

            do {
                var offset = (index + 6);
                var shift = (64 << index);

                value = (((value >> shift) & offset.NthFermatMask<T>()) | ((value & offset.NthFermatMask<T>()) << shift));
            } while (++index < limit);
        }

        return ((value >> (BinaryIntegerConstants<T>.Size >> 1)) | (value << (BinaryIntegerConstants<T>.Size >> 1)));
    }
    public static T ReverseDigits<T>(this T value) where T : IBinaryInteger<T> {
        var quotient = value;
        var result = T.Zero;

        do {
            (quotient, var remainder) = T.DivRem(left: quotient, right: T.CreateTruncating(value: 10U));

            result = ((result * T.CreateTruncating(value: 10U)) + remainder);
        } while (T.Zero < quotient);

        return result;
    }
}
