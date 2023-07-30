using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Ouroboros.Maths;
using System.Buffers.Binary;

#if DEBUG
for (var i = 3U; (i < (uint.MaxValue - 2U)); i += 2U) {
    uint modularInverse;
    uint reversedBits;
    uint reversedDigits;
    uint reversedEndianness;

    if (
        i.IsPrime() &&
        (modularInverse = i.ModularInverse()).IsPrime() &&
        (reversedBits = i.ReverseBits()).IsPrime() &&
        (reversedDigits = i.ReverseDigits()).IsPrime() &&
        (reversedEndianness = BinaryPrimitives.ReverseEndianness(value: i)).IsPrime()
    ) {
        Console.WriteLine(value: $"{i} | {modularInverse} | {reversedBits} | {reversedDigits} | {reversedEndianness}");

        break;
    }
}
#else
_ = BenchmarkRunner.Run<BinaryIntegerBenchmarks>();
#endif

[DisassemblyDiagnoser]
[MemoryDiagnoser]
public class BinaryIntegerBenchmarks
{
    private static Pcg32XshRr Rng { get; } = Pcg32XshRr.New();

    private static uint Value32 => Rng.NextUInt32();
    private static int Value32S => ((int)Rng.NextUInt32());

    private readonly Consumer m_consumer = new();

    [Benchmark]
    public ulong BitwisePair() => Value32.BitwisePair<uint, ulong>(other: Value32);
    [Benchmark]
    public long BitwisePairS() => Value32S.BitwisePair<int, long>(other: Value32S);
    [Benchmark]
    public uint ClearLowestSetBit() => Value32.ClearLowestSetBit();
    [Benchmark]
    public int ClearLowestSetBitS() => Value32S.ClearLowestSetBit();
    [Benchmark]
    public uint DigitalRoot() => Value32.DigitalRoot();
    [Benchmark]
    public int DigitalRootS() => Value32S.DigitalRoot();
    [Benchmark]
    public void EnumerateDigits() => Value32.EnumerateDigits().Consume(consumer: m_consumer);
    [Benchmark]
    public void EnumerateDigitsS() => Value32S.EnumerateDigits().Consume(consumer: m_consumer);
    [Benchmark]
    public uint ExtractLowestSetBit() => Value32.ExtractLowestSetBit();
    [Benchmark]
    public int ExtractLowestSetBitS() => Value32S.ExtractLowestSetBit();
    [Benchmark]
    public uint FillFromLowestClearBit() => Value32.FillFromLowestClearBit();
    [Benchmark]
    public int FillFromLowestClearBitS() => Value32S.FillFromLowestClearBit();
    [Benchmark]
    public uint FillFromLowestSetBit() => Value32.FillFromLowestSetBit();
    [Benchmark]
    public int FillFromLowestSetBitS() => Value32S.FillFromLowestSetBit();
    [Benchmark]
    public uint LeastSignificantBit() => Value32.LeastSignificantBit();
    [Benchmark]
    public int LeastSignificantBitS() => Value32S.LeastSignificantBit();
    [Benchmark]
    public uint LeastSignificantDigit() => Value32.LeastSignificantDigit();
    [Benchmark]
    public int LeastSignificantDigitS() => Value32S.LeastSignificantDigit();
    [Benchmark]
    public uint LogarithmBase10() => Value32.LogarithmBase10();
    [Benchmark]
    public int LogarithmBase10S() => Value32S.LogarithmBase10();
    [Benchmark]
    public uint MostSignificantBit() => Value32.MostSignificantBit();
    [Benchmark]
    public int MostSignificantBitS() => Value32S.MostSignificantBit();
    [Benchmark]
    public uint MostSignificantDigit() => Value32.MostSignificantDigit();
    [Benchmark]
    public int MostSignificantDigitS() => Value32S.MostSignificantDigit();
    [Benchmark(Baseline = true)]
    public uint NextInt32() => Value32;
    [Benchmark]
    public int NextInt32S() => Value32S;
    [Benchmark]
    public uint PermuteBitsLexicographically() => Value32.PermuteBitsLexicographically();
    [Benchmark]
    public int PermuteBitsLexicographicallyS() => Value32S.PermuteBitsLexicographically();
    [Benchmark]
    public uint ReflectedBinaryDecode() => Value32.ReflectedBinaryDecode();
    [Benchmark]
    public int ReflectedBinaryDecodeS() => Value32S.ReflectedBinaryDecode();
    [Benchmark]
    public uint ReflectedBinaryEncode() => Value32.ReflectedBinaryEncode();
    [Benchmark]
    public int ReflectedBinaryEncodeS() => Value32S.ReflectedBinaryEncode();
    [Benchmark]
    public uint ReverseBits() => Value32.ReverseBits();
    [Benchmark]
    public int ReverseBitsS() => Value32S.ReverseBits();
    [Benchmark]
    public uint ReverseDigits() => Value32.ReverseDigits();
    [Benchmark]
    public int ReverseDigitsS() => Value32S.ReverseDigits();
}
