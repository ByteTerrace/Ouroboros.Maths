using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Ouroboros.Maths;

#if DEBUG
for (var i = uint.MinValue; (i < uint.MaxValue); ++i) {
}
#else
_ = BenchmarkRunner.Run<Benchmarks>();
#endif

[DisassemblyDiagnoser]
[MemoryDiagnoser]
public class Benchmarks
{
    private static Pcg32XshRr Rng { get; } = Pcg32XshRr.New();

    public static uint[] Values { get; } = Enumerable.Range(count: 1, start: 0).Select(x => Rng.NextUInt32()).ToArray();

    private readonly Consumer m_consumer = new();

    //[ParamsSource(name: nameof(Values))]
    //public uint Value { get; set; }
    public uint Value => Rng.NextUInt32();

    //[Benchmark]
    public uint DigitalRoot() => Value.DigitalRoot();
    //[Benchmark]
    public void EnumerateDigits() => Value.EnumerateDigits().Consume(consumer: m_consumer);
    //[Benchmark]
    public bool IsPrime() => Value.IsPrime();
    //[Benchmark]
    public uint LeastSignificantDigit() => Value.LeastSignificantDigit();
    //[Benchmark]
    public uint Log10() => Value.Log10();
    //[Benchmark]
    public uint MostSignificantDigit() => Value.MostSignificantDigit();
    //[Benchmark]
    public uint ReverseBits() => Value.ReverseBits();
    //[Benchmark]
    public uint ReverseDigits() => Value.ReverseDigits();
    //[Benchmark]
    public uint SquareRoot() => Value.SquareRoot();
}
