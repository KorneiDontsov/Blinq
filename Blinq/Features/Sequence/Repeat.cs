namespace Blinq;

public struct RepeatIterator<T>: IIterator<T> {
   readonly T Item;
   int Count;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RepeatIterator (T item, int count) {
      Item = item;
      Count = count;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Count > 0) {
         --Count;
         item = Item;
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      while (Count > 0) {
         --Count;
         if (fold.Invoke(Item, ref seed)) break;
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Count;
      return true;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, RepeatIterator<T>> Repeat<T> (T item, int count) {
      if (count < 0) Get.Throw<ArgumentOutOfRangeException>();
      return new RepeatIterator<T>(item, count);
   }
}
