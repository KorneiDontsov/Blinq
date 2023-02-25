namespace Blinq.Collections;

// TODO: Implement all overloads

public struct ValueSetCollector<T, TEqualer>: ICollector<T, ValueSet<T, TEqualer>>
where T: notnull
where TEqualer: IEqualityComparer<T> {
   ValueSet<T, TEqualer> Set;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueSetCollector (TEqualer equaler) {
      Set = new(equaler);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal ValueSetCollector (ValueSet<T, TEqualer> emptySet) {
      Set = emptySet;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Set.EnsureCapacity(minCapacity);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      _ = Set.TryAdd(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueSet<T, TEqualer> Build () {
      return Set;
   }
}

public static class SetCollectors {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<ICollector<T, ValueSet<T, TEqualer>>, ValueSetCollector<T, TEqualer>> ValueSet<T, TEqualer> (
      this CollectorProvider<T> collectorProvider,
      TEqualer equaler
   )
   where T: notnull
   where TEqualer: IEqualityComparer<T> {
      _ = collectorProvider;
      return new ValueSetCollector<T, TEqualer>(equaler);
   }
}
