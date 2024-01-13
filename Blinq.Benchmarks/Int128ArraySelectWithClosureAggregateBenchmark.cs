using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int128ArraySelectWithClosureAggregateBenchmark {
   readonly Int128[] array =
      Enumerable.Range(0, 100_000)
         .Select(static number => (Int128)number)
         .ToArray();

   readonly Int128 additionalValue = 19;

   [Benchmark(Baseline = true)]
   public Int128 Linq () {
      var additionalValue = this.additionalValue;
      return this.array.Select(number => number * number + additionalValue)
         .Aggregate(Int128.Zero, static (a, b) => a + b);
   }

   [Benchmark]
   public Int128 Blinq () {
      var additionalValue = this.additionalValue;
      return this.array.Iterate()
         .Select(number => number * number + additionalValue)
         .Aggregate(Int128.Zero, static (a, b) => a + b);
   }

   [Benchmark]
   public Int128 BlinqWithClosure () {
      var additionalValue = this.additionalValue;
      return this.array.Iterate()
         .Select(
            Functor.New(
               static (Int128 additionalValue, Int128 number) => {
                  return number * number + additionalValue;
               }
            ).Bind(additionalValue)
         )
         .Aggregate(Int128.Zero, static (a, b) => a + b);
   }

   [Benchmark]
   public Int128 BlinqByRef () {
      var additionalValue = this.additionalValue;
      return this.array.Iterate()
         .Select((in Int128 number) => number * number + additionalValue)
         .Aggregate(Int128.Zero, static (in Int128 a, in Int128 b) => a + b);
   }

   [Benchmark]
   public Int128 BlinqByRefWithClosure () {
      var additionalValue = this.additionalValue;
      return this.array.Iterate()
         .Select(
            Functor.New(
               static (in Int128 additionalValue, in Int128 number) => {
                  return number * number + additionalValue;
               }
            ).Bind(additionalValue)
         )
         .Aggregate(Int128.Zero, static (in Int128 a, in Int128 b) => a + b);
   }

   [Benchmark]
   public Int128 ForEach () {
      var additionalValue = this.additionalValue;
      Int128 sum = 0;
      foreach (var number in this.array) {
         sum += number * number + additionalValue;
      }

      return sum;
   }

   [Benchmark]
   public Int128 For () {
      var additionalValue = this.additionalValue;
      Int128 sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         var number = this.array[index];
         sum += number * number + additionalValue;
      }

      return sum;
   }

   [Benchmark]
   public Int128 ForByRef () {
      var additionalValue = this.additionalValue;
      Int128 sum = 0;
      for (var index = 0; index < this.array.Length; index++) {
         ref readonly var number = ref this.array[index];
         sum += number * number + additionalValue;
      }

      return sum;
   }
}
