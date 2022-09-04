namespace Blinq;

struct ChunkFoldFunc<T, TCollection, TBuilder, TCollector>: IFoldFunc<T, (TBuilder builder, int countLeft)>
where TCollector: ICollector<T, TCollection, TBuilder> {
   TCollector Collector;

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
   TCollector Collector;
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
         Use<ICollector<T, TCollection, TBuilder>, TCollector> collectorUse
      )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection, TBuilder> {
      if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), size, null);

      var count = sequence.Count switch {
         (true, var inputCount) when System.Math.DivRem(inputCount, size, out var remainder) is var c => Option.Value(remainder is 0 ? c : c + 1),
         _ => Option.None,
      };
      var iterator = new ChunkIterator<TCollection, TBuilder, TCollector, T, TIterator>(sequence.Iterator, size, collectorUse.Value);
      return new Sequence<TCollection, ChunkIterator<TCollection, TBuilder, TCollector, T, TIterator>>(iterator, count);
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
