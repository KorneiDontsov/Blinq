using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public class IterateAsEnumerableBenchmarks {
   IEnumerable<int> Seq = null!;

   [Params(10, 100, 250)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Seq = Enumerable.Range(0, N);
   }

   [Benchmark]
   public int Query () {
      return Seq.Sum();
   }

   [Benchmark]
   public int Iterate_AsEnumerable_Query () {
      return Seq.Iterate().AsEnumerable().Sum();
   }

   [Benchmark]
   public int Foreach () {
      var sum = 0;
      foreach (var item in Seq) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public int Iterate_AsEnumerable_Foreach () {
      var sum = 0;
      foreach (var item in Seq.Iterate().AsEnumerable()) {
         sum += item;
      }

      return sum;
   }
}
