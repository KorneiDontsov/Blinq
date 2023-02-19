using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class WhereBenchmarks {
   static readonly Func<int, bool> Predicate = static number => number % 3 == 0;

   int[] Array = null!;

   [Params(10, 100, 200)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public int Loop_Where_Sum () {
      var sum = 0;
      foreach (var item in Array) {
         if (Predicate(item)) sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Linq_Where_Sum () {
      return Array.Where(Predicate).Sum();
   }

   [Benchmark]
   public long Blinq_Where_Sum () {
      return Array.Iter().Where(Predicate).Sum();
   }
}
