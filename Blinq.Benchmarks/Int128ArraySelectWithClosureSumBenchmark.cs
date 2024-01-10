using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int128ArraySelectWithClosureSumBenchmark {
   readonly Int128[] array =
      Enumerable.Range(0, 100_000)
         .Select(number => (Int128)number)
         .ToArray();

   readonly Int128 additionalValue = 19;

   [Benchmark(Baseline = true)]
   public Int128 For () {
      var additionalValue = this.additionalValue;
      Int128 sum = 0;
      for (var index = 0; index < array.Length; index++) {
         var number = array[index];
         checked {
            sum += number * number + additionalValue;
         }
      }

      return sum;
   }

   [Benchmark]
   public Int128 ForEach () {
      var additionalValue = this.additionalValue;
      Int128 sum = 0;
      foreach (var number in array) {
         checked {
            sum += number * number + additionalValue;
         }
      }

      return sum;
   }

   [Benchmark]
   public Int128 Linq () {
      var additionalValue = this.additionalValue;
      return array.Select(number => number * number + additionalValue)
         .Aggregate(Int128.Zero, (a, b) => a + b);
   }

   [Benchmark]
   public Int128 Blinq () {
      var additionalValue = this.additionalValue;
      return array.Iterate()
         .Select(number => number * number + additionalValue)
         .Sum();
   }

   [Benchmark]
   public Int128 BlinqByRef () {
      var additionalValue = this.additionalValue;
      return array.Iterate()
         .Select((in Int128 number) => number * number + additionalValue)
         .Sum();
   }

   [Benchmark]
   public Int128 BlinqByRefWithClosure () {
      var additionalValue = this.additionalValue;
      return array.Iterate()
         .Select(
            Functor.New(
               static (in Int128 number, in Int128 additionalValue) => {
                  return number * number + additionalValue;
               }
            ).Bind(additionalValue)
         )
         .Sum();
   }
}
