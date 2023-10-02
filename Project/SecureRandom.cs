using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Ouroboros.Maths;

public static class SecureRandom
{
    private static T NextUInt<T>(T exclusiveHigh) where T : struct, IBinaryInteger<T>, IUnsignedNumber<T> {
        var range = (T.AllBitsSet - (((T.AllBitsSet % exclusiveHigh) + T.One) % exclusiveHigh));
        T result;

        do {
            result = NextUInt<T>();
        } while (result > range);

        return (result % exclusiveHigh);
    }

    public static T NextUInt<T>() where T : struct, IBinaryInteger<T>, IUnsignedNumber<T> {
        var result = T.Zero;

        RandomNumberGenerator.Fill(data: MemoryMarshal.AsBytes(span: new Span<T>(reference: ref result)));

        return result;
    }
    public static T NextUInt<T>(T maximum, T minimum) where T : struct, IBinaryInteger<T>, IUnsignedNumber<T> {
        var range = (maximum - minimum);

        return ((range != T.AllBitsSet) ? (NextUInt(exclusiveHigh: (range + T.One)) + minimum) : NextUInt<T>());
    }
}
