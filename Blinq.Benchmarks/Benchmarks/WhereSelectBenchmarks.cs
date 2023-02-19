using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class WhereSelectBenchmarks {
   static readonly Func<int, long> Selector = static number => number * 2;
   static readonly Func<int, bool> Predicate = static number => number % 3 == 0;

   int[] Array = null!;

   [Params(10, 100, 250)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public long Loop_Where_Select_Sum () {
      var sum = 0L;
      foreach (var item in Array) {
         if (Predicate(item)) sum += Selector(item);
      }

      return sum;
   }

   [Benchmark]
   public long Linq_Where_Select_Sum () {
      return Array.Where(Predicate).Select(Selector).Sum();
   }

   [Benchmark]
   public long Blinq_Where_Select_Sum () {
      return Array.Iter().Where(Predicate).Select(Selector).Sum();
   }
}
