namespace Blinq;

readonly struct CollectFold<T, TCollection, TCollector>: IFold<T, TCollector>
where TCollector: ICollector<T, TCollection> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TCollector accumulator) {
      accumulator.Add(item);
      return false;
   }
}

public static partial class Iterator {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal static TCollection Collect<T, TIterator, TCollection, TCollector> (ref TIterator iterator, ref TCollector collector)
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      if (iterator.TryGetCount(out var count)) collector.EnsureCapacity(count);
      collector = iterator.Fold(collector, new CollectFold<T, TCollection, TCollector>());
      return collector.Build();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCollection Collect<T, TIterator, TCollection, TCollector> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Contract<ICollector<T, TCollection>, TCollector> collector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      var iter = iterator.Value;
      var coll = collector.Value;
      return Collect<T, TIterator, TCollection, TCollector>(ref iter, ref coll);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCollection Collect<T, TIterator, TCollection, TCollector> (
      this in Contract<IIterator<T>, TIterator> iterator,
      ProvideCollector<T, TCollection, TCollector> provideCollector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      return iterator.Collect(provideCollector.Invoke());
   }
}
