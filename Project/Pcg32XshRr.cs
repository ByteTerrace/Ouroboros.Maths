using System.Runtime.CompilerServices;

namespace Ouroboros.Maths;

public sealed class Pcg32XshRr
{
    private const ulong MAGIC_VALUE_DEFAULT = 6364136223846793005UL;
    private const string STREAM_VALUE_ERROR = "stream offset must be a positive integer less than 2^63";
    private const ulong STREAM_VALUE_MAX = ((1UL << 63) - 1UL);

    private static ulong Jump(ulong count, ulong magic, ulong state, ulong stream) {
        unchecked {
            var accMul = 1UL;
            var accAdd = 0UL;
            var curMul = magic;
            var curAdd = stream;

            while (count > 0UL) {
                if (0UL < (count & 1UL)) {
                    accMul *= curMul;
                    accAdd = ((accAdd * curMul) + curAdd);
                }

                curAdd = ((curMul + 1UL) * curAdd);
                curMul *= curMul;
                count >>= 1;
            }

            return ((accMul * state) + accAdd);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint NextUInt32(ulong magic, ref ulong state, ulong stream) {
        unchecked {
            state = ((state * magic) + stream);

            return uint.RotateRight(rotateAmount: ((int)(state >> 59)), value: ((uint)(((state >> 18) ^ state) >> 27)));
        }
    }
    private static uint Sample(uint exclusiveHigh, ulong magic, ref ulong state, ulong stream) {
        unchecked {
            var sample = ((ulong)NextUInt32(magic: magic, state: ref state, stream: stream));
            var result = (sample * exclusiveHigh);
            var leftover = ((uint)result);

            if (leftover < exclusiveHigh) {
                var threshold = ((((uint)(-exclusiveHigh)) % exclusiveHigh));

                while (leftover < threshold) {
                    sample = NextUInt32(magic: magic, state: ref state, stream: stream);
                    result = (sample * exclusiveHigh);
                    leftover = ((uint)result);
                }
            }

            return ((uint)(result >> 32));
        }
    }

    public static Pcg32XshRr New(ulong magic, ulong state, ulong stream) => new(magic: magic, state: state, stream: stream);
    public static Pcg32XshRr New(ulong state, ulong stream) => New(magic: MAGIC_VALUE_DEFAULT, state: state, stream: stream);
    public static Pcg32XshRr New() => New(state: SecureRandom.NextUInt64(), stream: SecureRandom.NextUInt64(maximum: STREAM_VALUE_MAX, minimum: 0UL));

    private readonly ulong m_magic;
    private readonly ulong m_stream;

    private ulong m_state;

    private Pcg32XshRr(ulong magic, ulong state, ulong stream) {
        if (stream > STREAM_VALUE_MAX) {
            throw new ArgumentOutOfRangeException(actualValue: stream, message: STREAM_VALUE_ERROR, paramName: nameof(stream));
        }

        m_magic = magic;
        m_state = state;
        m_stream = ((((~(stream & 1UL)) << 63) | stream) | 1UL);
    }

    public void Jump(ulong count) {
        m_state = Jump(count: count, magic: m_magic, state: m_state, stream: m_stream);
    }
    public uint NextPrime(uint maximum, uint minimum) {
        var result = (NextUInt32(maximum: maximum, minimum: minimum) | 1U);

        while (!result.IsPrime()) {
            result += 2U;
        }

        return result;
    }
    public uint NextUInt32() => NextUInt32(magic: m_magic, state: ref m_state, stream: m_stream);
    public uint NextUInt32(uint maximum, uint minimum) {
        var range = (maximum - minimum);

        return ((range != uint.MaxValue) ? (Sample(exclusiveHigh: (range + 1U), magic: m_magic, state: ref m_state, stream: m_stream) + minimum) : NextUInt32());
    }
}
