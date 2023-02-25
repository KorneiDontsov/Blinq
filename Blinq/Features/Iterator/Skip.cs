namespace Blinq;

readonly struct SkipFold<T>: IFold<T, int> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref int accumulator) {
      return --accumulator == 0;
   }
}

public struct SkipIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly int SkipCount;
   bool Skipped;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SkipIterator (TIterator iterator, int skipCount) {
      Iterator = iterator;
      SkipCount = skipCount;
      Skipped = SkipCount == 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   void EnsureSkipped () {
      if (!Skipped) {
         Iterator.Fold(SkipCount, new SkipFold<T>());
         Skipped = true;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      EnsureSkipped();
      return Iterator.TryPop(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      EnsureSkipped();
      return Iterator.Fold(accumulator, fold);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (Iterator.TryGetCount(out count)) {
         if (!Skipped) count = count <= SkipCount ? 0 : count - SkipCount;
         return true;
      } else {
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, SkipIterator<T, TIterator>> Skip<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, int count)
   where TIterator: IIterator<T> {
      if (count < 0) Get.Throw<ArgumentOutOfRangeException>();
      return new SkipIterator<T, TIterator>(iterator, count);
   }
}
