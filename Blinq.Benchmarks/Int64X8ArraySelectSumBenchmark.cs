using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int64X8ArraySelectSumBenchmark {
   readonly Int64X8[] array =
      Enumerable.Range(0, 100_000)
         .Select(number => new Int64X8 { Number1 = number })
         .ToArray();

   [Benchmark(Baseline = true)]
   public Int128 For () {
      Int128 sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         var number = this.array[index].Number1;
         checked {
            sum += number * number;
         }
      }

      return sum;
   }

   [Benchmark]
   public Int128 ForEach () {
      Int128 sum = 0;
      foreach (var item in this.array) {
         checked {
            sum += item.Number1 * item.Number1;
         }
      }

      return sum;
   }

   [Benchmark]
   public Int128 Linq () {
      return this.array.Select(item => item.Number1 * item.Number1)
         .Aggregate(Int128.Zero, (a, b) => a + b);
   }

   [Benchmark]
   public Int128 Blinq () {
      return this.array.Iterate().Select(item => item.Number1 * item.Number1).Sum();
   }

   [Benchmark]
   public Int128 BlinqByRef () {
      return this.array.Iterate().Select(
         (in Int64X8 item) => {
            var number = item.Number1;
            return number * number;
         }
      ).Sum();
   }
}
