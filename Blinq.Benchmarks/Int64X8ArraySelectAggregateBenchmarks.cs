using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int64X8ArraySelectAggregateBenchmarks {
   readonly Int64X8[] array =
      Enumerable.Range(0, 10_000)
         .Select(static number => new Int64X8 { Number1 = number, Number2 = number })
         .ToArray();

   [Benchmark(Baseline = true)]
   public Int128 Linq () {
      return this.array.Select(static item => item.Number1 * item.Number2)
         .Aggregate(0L, static (a, b) => a + b);
   }

   [Benchmark]
   public long Blinq () {
      return this.array.Iterate()
         .Select(static item => item.Number1 * item.Number2)
         .Aggregate(0L, static (a, b) => a + b);
   }

   [Benchmark]
   public long BlinqByRef () {
      return this.array.Iterate()
         .Select(static (in Int64X8 item) => item.Number1 * item.Number2)
         .Aggregate(0L, static (a, b) => a + b);
   }

   [Benchmark]
   public long ForEach () {
      var sum = 0L;
      foreach (var item in this.array) {
         sum += item.Number1 * item.Number2;
      }

      return sum;
   }

   [Benchmark]
   public long For () {
      var sum = 0L;
      for (var index = 0; index < this.array.Length; index++) {
         var item = this.array[index];
         sum += item.Number1 * item.Number2;
      }

      return sum;
   }

   [Benchmark]
   public long ForByRef () {
      var sum = 0L;
      for (var index = 0; index < this.array.Length; index++) {
         ref readonly var item = ref this.array[index];
         sum += item.Number1 * item.Number2;
      }

      return sum;
   }
}
