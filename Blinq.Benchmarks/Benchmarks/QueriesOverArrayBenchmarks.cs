using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
public class QueriesOverArrayBenchmarks {
   static readonly Func<int, long> Selector = number => number * 2;
   static readonly Func<int, bool> Predicate = number => number % 3 == 0;
   static readonly Func<long, bool> PostPredicate = number => number % 3 == 0;

   int[] Array = null!;

   [Params(10, 100, 250)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark]
   public int Foreach () {
      var sum = 0;
      foreach (var item in Array) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public int Iterate_Foreach () {
      var sum = 0;
      foreach (var item in Array.Iterate()) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Select () {
      var sum = 0L;
      foreach (var item in Array.Select(Selector)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Iterate_Select () {
      var sum = 0L;
      foreach (var item in Array.Iterate().Select(Selector)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Where () {
      var sum = 0;
      foreach (var item in Array.Where(Predicate)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Iterate_Where () {
      var sum = 0;
      foreach (var item in Array.Iterate().Where(Predicate)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Where_Select () {
      var sum = 0L;
      foreach (var item in Array.Where(Predicate).Select(Selector)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Iterate_Where_Select () {
      var sum = 0L;
      foreach (var item in Array.Iterate().Where(Predicate).Select(Selector)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Select_Where () {
      var sum = 0L;
      foreach (var item in Array.Select(Selector).Where(PostPredicate)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public long Iterate_Select_Where () {
      var sum = 0L;
      foreach (var item in Array.Iterate().Select(Selector).Where(PostPredicate)) {
         sum += item;
      }

      return sum;
   }

   [Benchmark]
   public int Aggregate () {
      return Array.Aggregate((a, b) => a + b);
   }

   [Benchmark]
   public int Iterate_Aggregate () {
      return Array.Iterate().Aggregate((a, b) => a + b).Value();
   }
}
