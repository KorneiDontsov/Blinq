using System;

namespace Blinq.CodeGen;

static class ArrayExtensions {
   public static ReadOnlyValueList<T> ToValueList<T> (this T[] array) {
      return array;
   }

   public static TResult[] ConvertAll<T, TResult> (this T[] array, Func<T, TResult> selector) {
      var resultArray = new TResult[array.Length];
      for (var index = 0; index < array.Length; index++) {
         resultArray[index] = selector(array[index]);
      }

      return resultArray;
   }
}
