namespace Blinq;

readonly struct TakeFold<T, TAccumulator, TInnerFold>: IFold<T, (TAccumulator accumulator, int countLeft)>
where TInnerFold: IFold<T, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TakeFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator accumulator, int countLeft) state) {
      --state.countLeft;
      return InnerFold.Invoke(item, ref state.accumulator) || state.countLeft == 0;
   }
}

public struct TakeIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   int TakeCount;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TakeIterator (TIterator iterator, int takeCount) {
      Iterator = iterator;
      TakeCount = takeCount;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (TakeCount > 0) {
         --TakeCount;
         return Iterator.TryPop(out item);
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      if (TakeCount > 0) {
         (seed, TakeCount) = Iterator.Fold((seed, TakeCount), new TakeFold<T, TAccumulator, TFold>(fold));
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (TakeCount == 0) {
         count = 0;
         return true;
      } else if (Iterator.TryGetCount(out count)) {
         if (count > TakeCount) count = TakeCount;
         return true;
      } else {
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, TakeIterator<T, TIterator>> Take<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, int count)
   where TIterator: IIterator<T> {
      if (count < 0) Get.Throw<ArgumentOutOfRangeException>();
      return new TakeIterator<T, TIterator>(iterator, count);
   }
}
