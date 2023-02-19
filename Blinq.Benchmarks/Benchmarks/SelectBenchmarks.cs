using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class SelectBenchmarks {
   static readonly Func<int, long> Selector = static number => number * 2;

   int[] Array = null!;

   [Params(10, 100, 200)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public long Loop_Select_Sum () {
      var sum = 0L;
      foreach (var item in Array) {
         sum += Selector(item);
      }

      return sum;
   }

   [Benchmark]
   public long Linq_Select_Sum () {
      return Array.Select(Selector).Sum();
   }

   [Benchmark]
   public long Blinq_Select_Sum () {
      return Array.Iter().Select(Selector).Sum();
   }
}
