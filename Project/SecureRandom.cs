using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Ouroboros.Maths;

public static class SecureRandom
{
    private static uint NextUInt32(uint exclusiveHigh) {
        var range = (uint.MaxValue - (((uint.MaxValue % exclusiveHigh) + 1U) % exclusiveHigh));
        uint result;

        do {
            result = NextUInt32();
        } while (result > range);

        return (result % exclusiveHigh);
    }
    private static ulong NextUInt64(ulong exclusiveHigh) {
        var range = (ulong.MaxValue - (((ulong.MaxValue % exclusiveHigh) + 1UL) % exclusiveHigh));
        ulong result;

        do {
            result = NextUInt64();
        } while (result > range);

        return (result % exclusiveHigh);
    }

    public static uint NextUInt32() {
        var result = 0U;

        RandomNumberGenerator.Fill(data: MemoryMarshal.AsBytes(span: new Span<uint>(reference: ref result)));

        return result;
    }
    public static uint NextUInt32(uint maximum, uint minimum) {
        var range = (maximum - minimum);

        return ((range != uint.MaxValue) ? (NextUInt32(exclusiveHigh: (range + 1U)) + minimum) : NextUInt32());
    }
    public static ulong NextUInt64() {
        var result = 0UL;

        RandomNumberGenerator.Fill(data: MemoryMarshal.AsBytes(span: new Span<ulong>(reference: ref result)));

        return result;
    }
    public static ulong NextUInt64(ulong maximum, ulong minimum) {
        var range = (maximum - minimum);

        return ((range != ulong.MaxValue) ? (NextUInt64(exclusiveHigh: (range + 1UL)) + minimum) : NextUInt64());
    }
}
