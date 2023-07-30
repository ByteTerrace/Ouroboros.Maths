using System.Numerics;

namespace Ouroboros.Maths;

public static class BinaryIntegerConstants<T> where T : IBinaryInteger<T>
{
    public static T Log2Size { get; }
    public static T Nine { get; }
    public static T Size { get; }
    public static T Ten { get; }

    static BinaryIntegerConstants() {
        var size = T.PopCount(value: T.AllBitsSet);

        Log2Size = T.Log2(value: size);
        Nine = T.CreateChecked(value: 9U);
        Size = size;
        Ten = T.CreateChecked(value: 10U);
    }
}
