using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int128ArraySelectAggregateBenchmarks {
   readonly Int128[] array =
      Enumerable.Range(0, 100_000)
         .Select(static number => (Int128)number)
         .ToArray();

   [Benchmark(Baseline = true)]
   public Int128 Linq () {
      return this.array.Select(static number => number * number)
         .Aggregate(Int128.Zero, static (a, b) => a + b);
   }

   [Benchmark]
   public Int128 Blinq () {
      return this.array.Iterate()
         .Select(static number => number * number)
         .Aggregate(Int128.Zero, static (a, b) => a + b);
   }

   [Benchmark]
   public Int128 BlinqByRef () {
      return this.array.Iterate()
         .Select(static (in Int128 number) => number * number)
         .Aggregate(Int128.Zero, static (a, b) => a + b);
   }

   [Benchmark]
   public Int128 ForEach () {
      Int128 sum = 0;
      foreach (var number in this.array) {
         sum += number * number;
      }

      return sum;
   }

   [Benchmark]
   public Int128 For () {
      Int128 sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         var number = this.array[index];
         sum += number * number;
      }

      return sum;
   }

   [Benchmark]
   public Int128 ForByRef () {
      Int128 sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         ref readonly var number = ref this.array[index];
         sum += number * number;
      }

      return sum;
   }
}
