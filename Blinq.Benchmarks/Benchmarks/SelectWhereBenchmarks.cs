using System.Linq;

namespace Blinq.Benchmarks;

[MemoryDiagnoser]
public class SelectWhereBenchmarks {
   static readonly Func<int, long> Selector = static number => number * 2;
   static readonly Func<long, bool> PostPredicate = static number => number % 3 == 0;
   int[] Array = null!;

   [Params(10, 100, 250)]
   public int N;

   [GlobalSetup]
   public void Setup () {
      Array = Utils.CreateArrayRange(N);
   }

   [Benchmark(Baseline = true)]
   public long Loop_Select_Where_Sum () {
      var sum = 0L;
      foreach (var item in Array) {
         var sItem = Selector(item);
         if (PostPredicate(sItem)) sum += sItem;
      }

      return sum;
   }

   [Benchmark]
   public long Linq_Select_Where_Sum () {
      return Array.Select(Selector).Where(PostPredicate).Sum();
   }

   [Benchmark]
   public long Blinq_Select_Where_Sum () {
      return Array.Seq().Select(Selector).Where(PostPredicate).Sum();
   }
}
