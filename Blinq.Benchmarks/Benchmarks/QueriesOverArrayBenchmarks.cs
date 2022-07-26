using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public class QueriesOverArrayBenchmarks {
   static readonly Func<int, long> Selector = number => number * 2;
   static readonly Func<int, bool> Predicate = number => number % 3 == 0;
   static readonly Func<long, bool> PostPredicate = number => number % 3 == 0;
   static readonly Func<int, int, int> AggregateFunc = (a, b) => a + b;

   int[] Array = null!;

   [Params(10, 100, 250)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark]
   public int SF_Sum () {
      var sum = 0;
      foreach (var item in Array) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public int Sum () {
      return Array.Sum();
   }

   [Benchmark]
   public int Iterate_Sum () {
      return Array.Iterate().Sum();
   }

   [Benchmark]
   public long SF_Select_Sum () {
      var sum = 0L;
      foreach (var item in Array) {
         sum += Selector(item);
      }

      return sum;
   }

   [Benchmark]
   public long Select_Sum () {
      return Array.Select(Selector).Sum();
   }

   [Benchmark]
   public long Iterate_Select_Sum () {
      return Array.Iterate().Select(Selector).Sum();
   }

   [Benchmark]
   public int SF_Where_Sum () {
      var sum = 0;
      foreach (var item in Array) {
         if (Predicate(item)) sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Where_Sum () {
      return Array.Where(Predicate).Sum();
   }

   [Benchmark]
   public long Iterate_Where_Sum () {
      return Array.Iterate().Where(Predicate).Sum();
   }

   [Benchmark]
   public long SF_Where_Select_Sum () {
      var sum = 0L;
      foreach (var item in Array) {
         if (Predicate(item)) sum += Selector(item);
      }

      return sum;
   }

   [Benchmark]
   public long Where_Select_Sum () {
      return Array.Where(Predicate).Select(Selector).Sum();
   }

   [Benchmark]
   public long Iterate_Where_Select_Sum () {
      return Array.Iterate().Where(Predicate).Select(Selector).Sum();
   }

   [Benchmark]
   public long SF_Select_Where_Sum () {
      var sum = 0L;
      foreach (var item in Array) {
         var sItem = Selector(item);
         if (PostPredicate(sItem)) sum += sItem;
      }

      return sum;
   }

   [Benchmark]
   public long Select_Where_Sum () {
      return Array.Select(Selector).Where(PostPredicate).Sum();
   }

   [Benchmark]
   public long Iterate_Select_Where_Sum () {
      return Array.Iterate().Select(Selector).Where(PostPredicate).Sum();
   }

   [Benchmark]
   public int SF_Aggregate () {
      var accumulated = 0;
      foreach (var item in Array) {
         accumulated = AggregateFunc(accumulated, item);
      }

      return accumulated;
   }

   [Benchmark]
   public int Aggregate () {
      return Array.Aggregate(AggregateFunc);
   }

   [Benchmark]
   public int Iterate_Aggregate () {
      return Array.Iterate().Aggregate(AggregateFunc).Value();
   }
}
