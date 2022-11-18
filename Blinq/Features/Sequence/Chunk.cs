namespace Blinq;

public struct ChunkIterator<T, TCollection, TCollector, TIterator>: IIterator<TCollection>
where TCollector: ICollector<T, TCollection>
where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly int Size;
   readonly ProvideCollector<T, TCollection, TCollector> ProvideCollector;
   bool Completed;

   public ChunkIterator (TIterator iterator, int size, ProvideCollector<T, TCollection, TCollector> provideCollector) {
      Iterator = iterator;
      Size = size;
      ProvideCollector = provideCollector;
      Completed = false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TCollection, TAccumulator> {
      while (!Completed) {
         var collector = ProvideCollector.Invoke().Value;
         collector.Capacity = Size;
         var collectFoldFunc = new CollectFoldFunc<T, TCollection, TCollector>();
         var takeFoldFunc = new TakeFoldFunc<T, TCollector, CollectFoldFunc<T, TCollection, TCollector>>(collectFoldFunc);
         (collector, var countLeft) = Iterator.Fold((collector, Size), takeFoldFunc);
         var collection = collector.Build();

         Completed = countLeft > 0;
         if (func.Invoke(collection, ref seed)) break;
      }

      return seed;
   }
}

public static partial class Sequence {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TCollection, ChunkIterator<T, TCollection, TCollector, TIterator>> Chunk<T, TIterator, TCollection, TCollector> (
      this in Sequence<T, TIterator> sequence,
      int size,
      ProvideCollector<T, TCollection, TCollector> provideCollector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      if (size <= 0) Get.Throw<ArgumentOutOfRangeException>();

      var newCount = sequence.Count switch {
         (true, var count) when System.Math.DivRem(count, size, out var rem) is var div => Option.Value(rem > 0 ? div + 1 : div),
         _ => Option.None,
      };
      var iterator = new ChunkIterator<T, TCollection, TCollector, TIterator>(sequence.Iterator, size, provideCollector);
      return Sequence<TCollection>.Create(iterator, newCount);
   }
}
