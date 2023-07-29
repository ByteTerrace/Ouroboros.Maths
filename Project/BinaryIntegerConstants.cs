using System.Numerics;

namespace Ouroboros.Maths;

public static class BinaryIntegerConstants<T> where T : IBinaryInteger<T>
{
    public static T Eleven { get; }
    public static T Five { get; }
    public static T Four { get; }
    public static T Log2Size { get; }
    public static T Seven { get; }
    public static T Seventeen { get; }
    public static T Six { get; }
    public static int Size { get; }
    public static T Ten { get; }
    public static T Thirteen { get; }
    public static T Three { get; }
    public static T Two { get; }

    static BinaryIntegerConstants() {
        var size = int.CreateChecked(value: T.PopCount(value: T.AllBitsSet));

        Eleven = T.CreateTruncating(value: 11U);
        Five = T.CreateTruncating(value: 5U);
        Four = T.CreateTruncating(value: 4U);
        Log2Size = T.Log2(value: T.CreateChecked(value: size));
        Six = T.CreateTruncating(value: 6U);
        Seven = T.CreateTruncating(value: 7U);
        Seventeen = T.CreateTruncating(value: 17U);
        Size = int.CreateChecked(value: size);
        Ten = T.CreateTruncating(value: 10U);
        Thirteen = T.CreateTruncating(value: 13U);
        Three = T.CreateTruncating(value: 3U);
        Two = T.CreateTruncating(value: 2U);
    }
}
