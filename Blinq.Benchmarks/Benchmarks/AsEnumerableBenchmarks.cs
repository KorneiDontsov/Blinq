using System.Collections.Generic;
using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class SeqAsEnumerableBenchmarks {
   IEnumerable<int> Range = null!;

   [Params(10, 100, 200)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Range = Enumerable.Range(0, N);
   }

   [Benchmark(Baseline = true)]
   public int Loop_Sum () {
      var sum = 0;
      foreach (var item in Range) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public int Linq_AsEnumerable_Sum () {
      return Range.AsEnumerable().Sum();
   }

   [Benchmark]
   public int Blinq_AsEnumerable_Sum () {
      return Range.Seq().AsEnumerable().Sum();
   }
}
