using Blinq.Collections;

namespace Blinq;

public struct ArrayCollector<T>: ICollector<T, T[]> {
   PoolingVector<T> Vector = new();

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ArrayCollector () { }

   public int Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Vector.Capacity = value; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      Vector.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T[] Build () {
      return Vector.MoveToArray();
   }
}

public static partial class Collectors {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<ICollector<T, T[]>, ArrayCollector<T>> Array<T> (this CollectorProvider<T> collectorProvider) {
      _ = collectorProvider;
      return new ArrayCollector<T>();
   }
}
