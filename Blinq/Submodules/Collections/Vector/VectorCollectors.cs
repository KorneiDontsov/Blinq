namespace Blinq.Collections;

public struct ValueVectorCollector<T>: ICollector<T, ValueVector<T>> {
   PoolingVector<T> Vector = new();

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueVectorCollector () { }

   public int Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Vector.Capacity = value; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      Vector.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueVector<T> Build () {
      return Vector.MoveToValueVector();
   }
}

public struct ImmVectorCollector<T>: ICollector<T, ImmVector<T>> {
   PoolingVector<T> Vector = new();

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ImmVectorCollector () { }

   public int Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Vector.Capacity = value; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      Vector.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ImmVector<T> Build () {
      return Vector.MoveToImmutableVector();
   }
}

public struct MutVectorCollector<T>: ICollector<T, MutVector<T>> {
   PoolingVector<T> Vector = new();

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public MutVectorCollector () { }

   public int Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Vector.Capacity = value; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      Vector.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public MutVector<T> Build () {
      return Vector.MoveToMutableVector();
   }
}

public static class VectorCollectors {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ValueVectorCollector<T> ValueVector<T> (this CollectorProvider<T> collectorProvider) {
      _ = collectorProvider;
      return new();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ImmVectorCollector<T> ImmutableVector<T> (this CollectorProvider<T> collectorProvider) {
      _ = collectorProvider;
      return new();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static MutVectorCollector<T> MutableVector<T> (this CollectorProvider<T> collectorProvider) {
      _ = collectorProvider;
      return new();
   }
}
