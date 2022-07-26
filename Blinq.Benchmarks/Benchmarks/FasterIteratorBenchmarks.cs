using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class FasterIteratorBenchmarks {
   static readonly Func<int, bool> Predicate = i => i % 2 == 0;
   int[] Array = null!;

   [Params(10, 100, 500)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = new int[N];
      for (var i = 0; i < N; ++i) {
         Array[i] = i;
      }
   }

   [Benchmark(Baseline = true)]
   public int StraightForward_ArrayWhereCount () {
      var count = 0;
      foreach (var item in Array) {
         if (Predicate(item)) ++count;
      }

      return count;
   }

   [Benchmark]
   public int Linq_ArrayWhereCount () {
      return Array.Where(Predicate).Count();
   }

   [Benchmark]
   public int Linq_ArrayCountWithPredicate () {
      return Array.Count(Predicate);
   }

   [Benchmark]
   public int Blinq_ArrayWhereCount () {
      return Array.Iterate().Where(Predicate).Count();
   }
}
