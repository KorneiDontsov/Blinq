using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int128ArraySelectSumBenchmark {
   readonly Int128[] array =
      Enumerable.Range(0, 100_000)
         .Select(number => (Int128)number)
         .ToArray();

   [Benchmark(Baseline = true)]
   public Int128 For () {
      Int128 sum = 0;
      for (var index = 0; index < array.Length; index++) {
         var number = array[index];
         checked {
            sum += number * number;
         }
      }

      return sum;
   }

   [Benchmark]
   public Int128 ForEach () {
      Int128 sum = 0;
      foreach (var number in array) {
         checked {
            sum += number * number;
         }
      }

      return sum;
   }

   [Benchmark]
   public Int128 Linq () {
      return array.Select(number => number * number).Aggregate(Int128.Zero, (a, b) => a + b);
   }

   [Benchmark]
   public Int128 Blinq () {
      return array.Iterate().Select(number => number * number).Sum();
   }

   [Benchmark]
   public Int128 BlinqByRef () {
      return array.Iterate().Select((in Int128 number) => number * number).Sum();
   }
}
