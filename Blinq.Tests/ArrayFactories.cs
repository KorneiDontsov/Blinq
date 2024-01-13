namespace Blinq.Tests;

interface IArrayFactory<T> {
   T[] GenerateArray (int length);
}

sealed class ObjectArrayFactory: IArrayFactory<object> {
   public static readonly ObjectArrayFactory shared = new();

   public object[] GenerateArray (int length) {
      var array = new object[length];
      for (var index = 0; index < array.Length; index++) {
         array[index] = new object();
      }

      return array;
   }
}

sealed class Int64ArrayFactory: IArrayFactory<long> {
   public static readonly Int64ArrayFactory shared = new();

   public long[] GenerateArray (int length) {
      var array = new long[length];
      for (var index = 0; index < array.Length; index++) {
         array[index] = (long)index * index;
      }

      return array;
   }
}
