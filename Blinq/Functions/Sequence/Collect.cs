namespace Blinq;

readonly struct CollectFoldFunc<T, TCollection, TBuilder, TCollector>: IFoldFunc<T, TBuilder>
where TCollector: ICollector<T, TCollection, TBuilder> {
   readonly TCollector Collector;

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
      Use<ICollector<T, TCollection, TBuilder>, TCollector> collector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection, TBuilder> {
      var builder = collector.Value.CreateBuilder(sequence.Count.OrDefault());
      try {
         builder = sequence.Iterator.Fold(builder, new CollectFoldFunc<T, TCollection, TBuilder, TCollector>(collector));
         return collector.Value.Build(ref builder);
      } finally {
         collector.Value.Finalize(ref builder);
      }
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
