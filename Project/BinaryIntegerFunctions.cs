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
        const int loopOffset = 7;

        int offset;
        int shift;

        var bitCountDividedByTwo = (int.CreateChecked(value: BinaryIntegerConstants<TResult>.Size) >> 1);
        var evenBits = TResult.CreateTruncating(value: other);
        var oddBits = TResult.CreateTruncating(value: value);

        if (loopOffset.NthPowerOfTwo<int>() < bitCountDividedByTwo) {
            var i = ((int.CreateChecked(value: BinaryIntegerConstants<TResult>.Log2Size) - loopOffset) - 1);

            do {
                offset = (i + (loopOffset - 1));
                shift = offset.NthPowerOfTwo<int>();

                DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift);
            } while (0 < --i);
        }

        offset = 6; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }
        offset = 5; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }
        offset = 4; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }
        offset = 3; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }
        offset = 2; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }
        offset = 1; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }
        offset = 0; if ((shift = offset.NthPowerOfTwo<int>()) < bitCountDividedByTwo) { DistributeBits(evenBits: ref evenBits, oddBits: ref oddBits, offset: offset, shift: shift); }

        return (oddBits | (evenBits << shift));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void DistributeBits(int offset, int shift, ref TResult evenBits, ref TResult oddBits) {
            var mask = offset.NthFermatMask<TResult>();

            evenBits = ((evenBits | (evenBits << shift)) & mask);
            oddBits = ((oddBits | (oddBits << shift)) & mask);
        }
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
        const int loopOffset = 8;

        var bitCount = int.CreateChecked(value: BinaryIntegerConstants<T>.Size);

        if (0.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 0.NthPowerOfTwo<int>()); }
        if (1.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 1.NthPowerOfTwo<int>()); }
        if (2.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 2.NthPowerOfTwo<int>()); }
        if (3.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 3.NthPowerOfTwo<int>()); }
        if (4.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 4.NthPowerOfTwo<int>()); }
        if (5.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 5.NthPowerOfTwo<int>()); }
        if (6.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 6.NthPowerOfTwo<int>()); }
        if (7.NthPowerOfTwo<int>() < bitCount) { value ^= (value >> 7.NthPowerOfTwo<int>()); }

        if (loopOffset.NthPowerOfTwo<int>() < bitCount) {
            var i = (int.CreateChecked(value: BinaryIntegerConstants<T>.Log2Size) - loopOffset);

            do {
                value ^= (value >> (bitCount >> i));
            } while (0 < --i);
        }

        return value;
    }
    public static T ReflectedBinaryEncode<T>(this T value) where T : IBinaryInteger<T> =>
        (value ^ (value >> 1));
    public static T ReverseBits<T>(this T value) where T : IBinaryInteger<T> {
        const int loopOffset = 7;

        int offset;

        var bitCountDividedByTwo = (int.CreateChecked(value: BinaryIntegerConstants<T>.Size) >> 1);

        offset = 0; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }
        offset = 1; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }
        offset = 2; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }
        offset = 3; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }
        offset = 4; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }
        offset = 5; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }
        offset = 6; if (offset.NthPowerOfTwo<int>() < bitCountDividedByTwo) { SwapBitPairs(offset: offset, value: ref value); }

        if (loopOffset.NthPowerOfTwo<int>() < bitCountDividedByTwo) {
            var i = ((int.CreateChecked(value: BinaryIntegerConstants<T>.Log2Size) - loopOffset) - 1);

            do {
                SwapBitPairs(offset: offset++, value: ref value);
            } while (0 < --i);
        }

        return ((value >> bitCountDividedByTwo) | (value << bitCountDividedByTwo));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SwapBitPairs(int offset, ref T value) {
            var mask = offset.NthFermatMask<T>();
            var shift = offset.NthPowerOfTwo<int>();

            value = (((value >> shift) & mask) | ((value & mask) << shift));
        }
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
