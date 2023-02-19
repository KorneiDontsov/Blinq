using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class WhereCountBenchmarks {
   static readonly Func<int, bool> Predicate = static i => i % 2 == 0;
   int[] Array = null!;

   [Params(10, 100, 500)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public int Loop_Where_Count () {
      var count = 0;
      foreach (var item in Array) {
         if (Predicate(item)) ++count;
      }

      return count;
   }

   [Benchmark]
   public int Linq_Where_Count () {
      return Array.Where(Predicate).Count();
   }

   [Benchmark]
   public int Linq_CountWithPredicate () {
      return Array.Count(Predicate);
   }

   [Benchmark]
   public int Blinq_Where_Count () {
      return Array.Iter().Where(Predicate).Count();
   }
}
