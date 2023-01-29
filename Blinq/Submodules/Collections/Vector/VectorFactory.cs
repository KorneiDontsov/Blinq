namespace Blinq.Collections;

public static class VectorFactory {
   public static ValueVector<T> ToValueVector<T> (this ICollection<T> collection) {
      var count = collection.Count;
      if (count > 0) {
         var array = new T[collection.Count];
         collection.CopyTo(array, 0);
         return new(array);
      } else {
         return new();
      }
   }

   public static ImmVector<T> ToImmutableVector<T> (this ICollection<T> collection) {
      return ImmVector<T>.Create(collection.ToValueVector());
   }

   public static MutVector<T> ToMutableVector<T> (this ICollection<T> collection) {
      return new(collection.ToValueVector());
   }

   public static ValueVector<T> ToValueVector<T> (this IEnumerable<T> enumerable) {
      if (enumerable is ICollection<T> collection) {
         return collection.ToValueVector();
      } else {
         var poolingVector = new PoolingVector<T>();
         using (var en = enumerable.GetEnumerator()) {
            while (en.MoveNext()) poolingVector.Add(en.Current);
         }

         return poolingVector.MoveToValueVector();
      }
   }

   public static ImmVector<T> ToImmutableVector<T> (this IEnumerable<T> enumerable) {
      return ImmVector<T>.Create(enumerable.ToValueVector());
   }

   public static MutVector<T> ToMutableVector<T> (this IEnumerable<T> enumerable) {
      return new(enumerable.ToValueVector());
   }
}
