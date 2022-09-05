namespace Blinq;

readonly struct ChunkFoldFunc<T, TCollection, TBuilder, TCollector>: IFoldFunc<T, (TBuilder builder, int countLeft)>
where TCollector: ICollector<T, TCollection, TBuilder> {
   readonly TCollector Collector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ChunkFoldFunc (TCollector collector) {
      Collector = collector;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TBuilder builder, int countLeft) accumulator) {
      Collector.Add(ref accumulator.builder, item);
      return --accumulator.countLeft == 0;
   }
}

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
         var builder = Collector.CreateBuilder();
         (builder, var countLeft) = Iterator.Fold((builder, Size), new ChunkFoldFunc<T, TCollection, TBuilder, TCollector>(Collector));
         var collection = Collector.Build(builder);

         Completed = countLeft > 0;
         if (func.Invoke(collection, ref seed)) break;
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
      if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), size, null);

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
