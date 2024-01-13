using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int64ArraySelectAggregateBenchmark {
   readonly long[] array =
      Enumerable.Range(0, 100_000)
         .Select(static number => (long)number)
         .ToArray();

   [Benchmark(Baseline = true)]
   public long Linq () {
      return this.array.Select(static number => number * number)
         .Aggregate(0L, static (a, b) => a + b);
   }

   [Benchmark]
   public long Blinq () {
      return this.array.Iterate()
         .Select(static number => number * number)
         .Aggregate(0L, static (a, b) => a + b);
   }

   [Benchmark]
   public long BlinqByRef () {
      return this.array.Iterate()
         .Select(static (in long number) => number * number)
         .Aggregate(0L, static (a, b) => a + b);
   }

   [Benchmark]
   public long ForEach () {
      long sum = 0;
      foreach (var number in this.array) {
         sum += number * number;
      }

      return sum;
   }

   [Benchmark]
   public long For () {
      long sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         var number = this.array[index];
         sum += number * number;
      }

      return sum;
   }

   [Benchmark]
   public long ForByRef () {
      long sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         ref readonly var number = ref this.array[index];
         sum += number * number;
      }

      return sum;
   }
}
