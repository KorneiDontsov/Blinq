namespace Blinq.Benchmarks;

public static class Utils {
   public static int[] CreateArrayRange (int start, int count) {
      var array = new int[count];
      foreach (ref var item in array.AsSpan()) {
         item = start++;
      }

      return array;
   }

   public static int[] CreateArrayRange (int n) {
      return CreateArrayRange(0, n);
   }
}
