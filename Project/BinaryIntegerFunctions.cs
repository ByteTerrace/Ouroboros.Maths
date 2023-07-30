using System.Numerics;
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
    internal static T NthFermatMask<T>(this int value) where T : IBinaryInteger<T> {
        var x = T.AllBitsSet;
        var y = T.IsNegative(value: x).As<int>();

        return ((((x >>> y) / value.NthFermatNumber<T>()) << y) | T.One);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T NthFermatNumber<T>(this int value) where T : IBinaryInteger<T> =>
        ((T.One << (1 << value)) + T.One);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T NthPowerOfTwo<T>(this int value) where T : IBinaryInteger<T> =>
        (T.One << value);

    public static TResult BitwisePair<TInput, TResult>(this TInput value, TInput other) where TInput : IBinaryInteger<TInput> where TResult : IBinaryInteger<TResult> {
        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<TResult>.Size);
        var x = TResult.CreateTruncating(value: value);
        var y = TResult.CreateTruncating(value: other);

        if (bitCount > 7.NthPowerOfTwo<int>()) {
            var i = (int.Log2(value: bitCount) - 7);

            do {
                x = ((x | (x << i.NthPowerOfTwo<int>())) & i.NthFermatMask<TResult>());
                y = ((y | (y << i.NthPowerOfTwo<int>())) & i.NthFermatMask<TResult>());
            } while (0 < --i);
        }

        if (bitCount > 6.NthPowerOfTwo<int>()) {
            x = ((x | (x << 6.NthPowerOfTwo<int>())) & 6.NthFermatMask<TResult>());
            y = ((y | (y << 6.NthPowerOfTwo<int>())) & 6.NthFermatMask<TResult>());
        }

        if (bitCount > 5.NthPowerOfTwo<int>()) {
            x = ((x | (x << 5.NthPowerOfTwo<int>())) & 5.NthFermatMask<TResult>());
            y = ((y | (y << 5.NthPowerOfTwo<int>())) & 5.NthFermatMask<TResult>());
        }

        if (bitCount > 4.NthPowerOfTwo<int>()) {
            x = ((x | (x << 4.NthPowerOfTwo<int>())) & 4.NthFermatMask<TResult>());
            y = ((y | (y << 4.NthPowerOfTwo<int>())) & 4.NthFermatMask<TResult>());
        }

        if (bitCount > 3.NthPowerOfTwo<int>()) {
            x = ((x | (x << 3.NthPowerOfTwo<int>())) & 3.NthFermatMask<TResult>());
            y = ((y | (y << 3.NthPowerOfTwo<int>())) & 3.NthFermatMask<TResult>());
        }

        if (bitCount > 2.NthPowerOfTwo<int>()) {
            x = ((x | (x << 2.NthPowerOfTwo<int>())) & 2.NthFermatMask<TResult>());
            y = ((y | (y << 2.NthPowerOfTwo<int>())) & 2.NthFermatMask<TResult>());
        }

        if (bitCount > 1.NthPowerOfTwo<int>()) {
            x = ((x | (x << 1.NthPowerOfTwo<int>())) & 1.NthFermatMask<TResult>());
            y = ((y | (y << 1.NthPowerOfTwo<int>())) & 1.NthFermatMask<TResult>());
        }

        x = ((x | (x << 0.NthPowerOfTwo<int>())) & 0.NthFermatMask<TResult>());
        y = ((y | (y << 0.NthPowerOfTwo<int>())) & 0.NthFermatMask<TResult>());

        return (x | (y << 0.NthPowerOfTwo<int>()));
    }
    public static T ClearLowestSetBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value & (value - T.One));
    public static T DigitalRoot<T>(this T value) where T : IBinaryInteger<T> {
        var x = value.IsNonZero();
        var y = T.Abs(value: value);
        var z = BinaryIntegerConstants<T>.Nine;

        return (x + ((y - x) % z));
    }
    public static IEnumerable<T> EnumerateDigits<T>(this T value) where T : IBinaryInteger<T> {
        var quotient = T.Abs(value: value);

        do {
            (quotient, var remainder) = T.DivRem(left: quotient, right: BinaryIntegerConstants<T>.Ten);

            yield return remainder;
        } while (T.Zero < quotient);
    }
    public static T Exponentiate<T>(this T value, T exponent) where T : IBinaryInteger<T> {
        var result = T.One;

        do {
            if (T.IsOddInteger(value: exponent)) {
                result *= value;
            }

            exponent >>= 1;
            value *= value;
        } while (T.Zero < exponent);

        return result;
    }
    public static T ExtractLowestSetBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value & (-value));
    public static T FillFromLowestClearBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value & (value + T.One));
    public static T FillFromLowestSetBit<T>(this T value) where T : IBinaryInteger<T> =>
        (value | (value - T.One));
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
        (value % BinaryIntegerConstants<T>.Ten);
    public static T LogarithmBase10<T>(this T value) where T : IBinaryInteger<T> {
        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);

        value = T.Abs(value: value);

        return bitCount switch {
#if !FORCE_SOFTWARE_LOG10
            8 => (T.CreateTruncating(value: ((uint)MathF.Log10(x: uint.CreateTruncating(value: value)))) + T.One),
            16 => (T.CreateTruncating(value: ((uint)MathF.Log10(x: uint.CreateTruncating(value: value)))) + T.One),
            32 => (T.CreateTruncating(value: ((uint)Math.Log10(d: uint.CreateTruncating(value: value)))) + T.One),
#endif
            _ => SoftwareImplementation(value: value),
        };

        static T SoftwareImplementation(T value) {
            var quotient = value;
            var result = T.Zero;

            do {
                quotient /= BinaryIntegerConstants<T>.Ten;
                ++result;
            } while (T.Zero < quotient);

            return result;
        }
    }
    public static T MostSignificantBit<T>(this T value) where T : IBinaryInteger<T> =>
        (BinaryIntegerConstants<T>.Size - T.LeadingZeroCount(value: value));
    public static T MostSignificantDigit<T>(this T value) where T : IBinaryInteger<T> =>
        (value / BinaryIntegerConstants<T>.Ten.Exponentiate(exponent: (value.LogarithmBase10() - T.One)));
    public static T PermuteBitsLexicographically<T>(this T value) where T : IBinaryInteger<T> {
        var x = value.FillFromLowestSetBit();
        var y = int.CreateTruncating(value: (T.TrailingZeroCount(value: value) + T.One));
        var z = (((~x).ExtractLowestSetBit() - T.One) >> y);

        return ((x + T.One) | z);
    }
    public static T PopulationParity<T>(this T value) where T : IBinaryInteger<T> =>
        (T.PopCount(value: value) & T.One);
    public static T ReflectedBinaryDecode<T>(this T value) where T : IBinaryInteger<T> {
        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);

        value ^= (value >> 0.NthPowerOfTwo<int>());

        if (bitCount > 1.NthPowerOfTwo<int>()) {
            value ^= (value >> 1.NthPowerOfTwo<int>());
        }

        if (bitCount > 2.NthPowerOfTwo<int>()) {
            value ^= (value >> 2.NthPowerOfTwo<int>());
        }

        if (bitCount > 3.NthPowerOfTwo<int>()) {
            value ^= (value >> 3.NthPowerOfTwo<int>());
        }

        if (bitCount > 4.NthPowerOfTwo<int>()) {
            value ^= (value >> 4.NthPowerOfTwo<int>());
        }

        if (bitCount > 5.NthPowerOfTwo<int>()) {
            value ^= (value >> 5.NthPowerOfTwo<int>());
        }

        if (bitCount > 6.NthPowerOfTwo<int>()) {
            value ^= (value >> 6.NthPowerOfTwo<int>());
        }

        if (bitCount > 7.NthPowerOfTwo<int>()) {
            var i = (int.Log2(value: bitCount) - 7);

            do {
                value ^= (value >> (bitCount >> i));
            } while (0 < --i);
        }

        return value;
    }
    public static T ReflectedBinaryEncode<T>(this T value) where T : IBinaryInteger<T> =>
        (value ^ (value >> 1));
    public static T ReverseBits<T>(this T value) where T : IBinaryInteger<T> {
        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);

        if (bitCount > 1.NthPowerOfTwo<int>()) {
            value = (((value >> 0.NthPowerOfTwo<int>()) & 0.NthFermatMask<T>()) | ((value & 0.NthFermatMask<T>()) << 0.NthPowerOfTwo<int>()));
        }

        if (bitCount > 2.NthPowerOfTwo<int>()) {
            value = (((value >> 1.NthPowerOfTwo<int>()) & 1.NthFermatMask<T>()) | ((value & 1.NthFermatMask<T>()) << 1.NthPowerOfTwo<int>()));
        }

        if (bitCount > 3.NthPowerOfTwo<int>()) {
            value = (((value >> 2.NthPowerOfTwo<int>()) & 2.NthFermatMask<T>()) | ((value & 2.NthFermatMask<T>()) << 2.NthPowerOfTwo<int>()));
        }

        if (bitCount > 4.NthPowerOfTwo<int>()) {
            value = (((value >> 3.NthPowerOfTwo<int>()) & 3.NthFermatMask<T>()) | ((value & 3.NthFermatMask<T>()) << 3.NthPowerOfTwo<int>()));
        }

        if (bitCount > 5.NthPowerOfTwo<int>()) {
            value = (((value >> 4.NthPowerOfTwo<int>()) & 4.NthFermatMask<T>()) | ((value & 4.NthFermatMask<T>()) << 4.NthPowerOfTwo<int>()));
        }

        if (bitCount > 6.NthPowerOfTwo<int>()) {
            value = (((value >> 5.NthPowerOfTwo<int>()) & 5.NthFermatMask<T>()) | ((value & 5.NthFermatMask<T>()) << 5.NthPowerOfTwo<int>()));
        }

        if (bitCount > 7.NthPowerOfTwo<int>()) {
            var index = 0;
            var limit = (int.Log2(value: bitCount) - 7);

            do {
                var mask = (index + 6).NthFermatMask<T>();
                var shift = (6.NthPowerOfTwo<int>() << index);

                value = (((value >> shift) & mask) | ((value & mask) << shift));
            } while (++index < limit);
        }

        return ((value >> (bitCount >> 1)) | (value << (bitCount >> 1)));
    }
    public static T ReverseDigits<T>(this T value) where T : IBinaryInteger<T> {
        var quotient = T.Abs(value: value);
        var result = T.Zero;

        do {
            (quotient, var remainder) = T.DivRem(left: quotient, right: BinaryIntegerConstants<T>.Ten);

            result = ((result * BinaryIntegerConstants<T>.Ten) + remainder);
        } while (T.Zero < quotient);

        return T.CopySign(sign: value, value: result);
    }
}
