using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class Int64ArraySelectSumBenchmark {
   readonly long[] array =
      Enumerable.Range(0, 100_000)
         .Select(number => (long)number)
         .ToArray();

   [Benchmark(Baseline = true)]
   public long For () {
      long sum = 0;
      for (var index = 0; index < array.Length; index++) {
         var number = array[index];
         checked {
            sum += number * number;
         }
      }

      return sum;
   }

   [Benchmark]
   public long ForEach () {
      long sum = 0;
      foreach (var number in array) {
         checked {
            sum += number * number;
         }
      }

      return sum;
   }

   [Benchmark]
   public long Linq () {
      return array.Select(number => number * number).Sum();
   }

   [Benchmark]
   public long Blinq () {
      return array.Iterate().Select(number => number * number).Sum();
   }
}
