namespace Blinq;

struct CollectFoldFunc<T, TCollection, TBuilder, TCollector>: IFoldFunc<T, TBuilder>
where TCollector: ICollector<T, TCollection, TBuilder> {
   TCollector Collector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CollectFoldFunc (TCollector collector) {
      Collector = collector;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TBuilder accumulator) {
      Collector.Add(ref accumulator, item);
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCollection Collect<T, TIterator, TCollection, TBuilder, TCollector> (
      this in Sequence<T, TIterator> sequence,
      Use<ICollector<T, TCollection, TBuilder>, TCollector> collectorUse
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection, TBuilder> {
      var collector = collectorUse.Value;
      var builder = sequence.Iterator.Fold(collector.CreateBuilder(), new CollectFoldFunc<T, TCollection, TBuilder, TCollector>(collector));
      return collector.Build(builder);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCollection Collect<T, TIterator, TCollection, TBuilder, TCollector> (
      this in Sequence<T, TIterator> sequence,
      ProvideCollector<T, TCollection, TBuilder, TCollector> provideCollector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection, TBuilder> {
      return sequence.Collect(provideCollector.Invoke());
   }
}
