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

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TCollection item) {
      if (Completed) {
         item = default;
         return false;
      } else {
         var collector = ProvideCollector().Value;
         collector.EnsureCapacity(Size);
         var collectFold = new CollectFold<T, TCollection, TCollector>();
         var takeThenCollectFold = new TakeFold<T, TCollector, CollectFold<T, TCollection, TCollector>>(collectFold);
         (collector, var countLeft) = Iterator.Fold((collector, Size), takeThenCollectFold);
         Completed = countLeft > 0;
         item = collector.Build();
         return true;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<TCollection, TAccumulator> {
      while (TryPop(out var item) && !fold.Invoke(item, ref seed)) { }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (Iterator.TryGetCount(out count)) {
         var div = Math.DivRem(count, Size, out var rem);
         count = rem > 0 ? div + 1 : div;
         return true;
      } else {
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TCollection>, ChunkIterator<T, TCollection, TCollector, TIterator>> Chunk<T, TIterator, TCollection, TCollector> (
      this in Contract<IIterator<T>, TIterator> iterator,
      int size,
      ProvideCollector<T, TCollection, TCollector> provideCollector
   )
   where TIterator: IIterator<T>
   where TCollector: ICollector<T, TCollection> {
      if (size <= 0) Get.Throw<ArgumentOutOfRangeException>();
      return new ChunkIterator<T, TCollection, TCollector, TIterator>(iterator, size, provideCollector);
   }
}
