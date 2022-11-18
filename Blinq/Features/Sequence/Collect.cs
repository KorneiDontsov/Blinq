namespace Blinq;

readonly struct CollectFoldFunc<T, TCollection, TCollector>: IFoldFunc<T, TCollector>
where TCollector: ICollector<T, TCollection> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TCollector accumulator) {
      accumulator.Add(item);
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCollection Collect<T, TIterator, TCollection, TCollector> (
      this in Sequence<T, TIterator> sequence,
      Contract<ICollector<T, TCollection>, TCollector> collector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      var collectorValue = collector.Value;
      if (sequence.Count is (true, var count)) collectorValue.Capacity = count;
      collectorValue = sequence.Iterator.Fold(collectorValue, new CollectFoldFunc<T, TCollection, TCollector>());
      return collectorValue.Build();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCollection Collect<T, TIterator, TCollection, TCollector> (
      this in Sequence<T, TIterator> sequence,
      ProvideCollector<T, TCollection, TCollector> provideCollector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      return sequence.Collect(provideCollector.Invoke());
   }
}
