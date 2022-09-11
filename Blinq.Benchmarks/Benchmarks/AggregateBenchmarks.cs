using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public class AggregateBenchmarks {
   static readonly Func<int, int, int> AccumulatorFunc = static (a, b) => a + b;

   int[] Array = null!;

   [Params(10, 100, 200)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public int Loop_Aggregate () {
      var accumulator = 0;
      foreach (var item in Array) {
         accumulator = AccumulatorFunc(accumulator, item);
      }

      return accumulator;
   }

   [Benchmark]
   public int Aggregate () {
      return Array.Aggregate(AccumulatorFunc);
   }

   [Benchmark]
   public int Blinq_Aggregate () {
      return Array.Seq().Aggregate(AccumulatorFunc).Value();
   }
}
