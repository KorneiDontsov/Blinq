using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class SumBenchmarks {
   int[] Array = null!;

   [Params(10, 100, 200)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public int Loop_Sum () {
      var sum = 0;
      foreach (var item in Array) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public int Linq_Sum () {
      return Array.Sum();
   }

   [Benchmark]
   public int Blinq_Sum () {
      return Array.Iter().Sum();
   }
}
