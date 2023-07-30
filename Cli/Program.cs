using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Ouroboros.Maths;

#if DEBUG
for (var i = uint.MinValue; (i < uint.MaxValue); ++i) {
}
#else
_ = BenchmarkRunner.Run<BinaryIntegerBenchmarks>();
#endif

[DisassemblyDiagnoser]
[MemoryDiagnoser]
public class BinaryIntegerBenchmarks
{
    private static Pcg32XshRr Rng { get; } = Pcg32XshRr.New();

    private static uint Value => Rng.NextUInt32();

    private readonly Consumer m_consumer = new();

    [Benchmark]
    public ulong BitwisePair() => Value.BitwisePair<uint, ulong>(other: Value);
    [Benchmark]
    public uint ClearLowestSetBit() => Value.ClearLowestSetBit();
    [Benchmark]
    public uint DigitalRoot() => Value.DigitalRoot();
    [Benchmark]
    public void EnumerateDigits() => Value.EnumerateDigits().Consume(consumer: m_consumer);
    [Benchmark]
    public uint ExtractLowestSetBit() => Value.ExtractLowestSetBit();
    [Benchmark]
    public uint LeastSignificantBit() => Value.LeastSignificantBit();
    [Benchmark]
    public uint LeastSignificantDigit() => Value.LeastSignificantDigit();
    [Benchmark]
    public uint MostSignificantBit() => Value.MostSignificantBit();
    [Benchmark]
    public uint MostSignificantDigit() => Value.MostSignificantDigit();
    [Benchmark(Baseline = true)]
    public uint NextUInt32() => Value;
    [Benchmark]
    public uint PermuteBitsLexicographically() => Value.PermuteBitsLexicographically();
    [Benchmark]
    public uint ReflectedBinaryDecode() => Value.ReflectedBinaryDecode();
    [Benchmark]
    public uint ReflectedBinaryEncode() => Value.ReflectedBinaryEncode();
    [Benchmark]
    public uint ReverseBits() => Value.ReverseBits();
    [Benchmark]
    public uint ReverseDigits() => Value.ReverseDigits();
}
