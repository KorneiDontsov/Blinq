namespace Blinq;

public struct ChunkIterator<TCollection, TBuilder, TCollector, T, TIterator>: IIterator<TCollection>
where TCollector: ICollector<T, TCollection, TBuilder>
where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly int Size;
   readonly TCollector Collector;
   bool Completed;

   public ChunkIterator (TIterator iterator, int size, TCollector collector) {
      Iterator = iterator;
      Size = size;
      Collector = collector;
      Completed = false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TCollection, TAccumulator> {
      while (!Completed) {
         var builder = Collector.CreateBuilder(Size);
         try {
            var collectFoldFunc = new CollectFoldFunc<T, TCollection, TBuilder, TCollector>(Collector);
            var takeFoldFunc = new TakeFoldFunc<T, TBuilder, CollectFoldFunc<T, TCollection, TBuilder, TCollector>>(collectFoldFunc);
            (builder, var countLeft) = Iterator.Fold((builder, Size), takeFoldFunc);
            var collection = Collector.Build(ref builder);

            Completed = countLeft > 0;
            if (func.Invoke(collection, ref seed)) break;
         } finally {
            Collector.Finalize(ref builder);
         }
      }

      return seed;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TCollection, ChunkIterator<TCollection, TBuilder, TCollector, T, TIterator>>
      Chunk<T, TIterator, TBuilder, TCollection, TCollector> (
         this in Sequence<T, TIterator> sequence,
         int size,
         Use<ICollector<T, TCollection, TBuilder>, TCollector> collector
      )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection, TBuilder> {
      if (size <= 0) Utils.Throw<ArgumentOutOfRangeException>();

      var newCount = sequence.Count switch {
         (true, var count) when System.Math.DivRem(count, size, out var rem) is var div => Option.Value(rem > 0 ? div + 1 : div),
         _ => Option.None,
      };
      var iterator = new ChunkIterator<TCollection, TBuilder, TCollector, T, TIterator>(sequence.Iterator, size, collector.Value);
      return Sequence<TCollection>.Create(iterator, newCount);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TCollection, ChunkIterator<TCollection, TBuilder, TCollector, T, TIterator>>
      Chunk<T, TIterator, TBuilder, TCollection, TCollector> (
         this in Sequence<T, TIterator> sequence,
         int size,
         ProvideCollector<T, TCollection, TBuilder, TCollector> provideCollector
      )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection, TBuilder> {
      return sequence.Chunk(size, provideCollector.Invoke());
   }
}
