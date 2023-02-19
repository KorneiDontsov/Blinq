namespace Blinq;

public struct ConcatIterator<TOut, TIterator1, TIterator2>: IIterator<TOut>
where TIterator1: IIterator<TOut>
where TIterator2: IIterator<TOut> {
   TIterator1 Iterator1;
   TIterator2 Iterator2;
   bool OnIterator1 = true;

   public ConcatIterator (TIterator1 iterator1, TIterator2 iterator2) {
      Iterator1 = iterator1;
      Iterator2 = iterator2;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      if (OnIterator1) {
         if (Iterator1.TryPop(out item)) return true;
         OnIterator1 = false;
      }

      return Iterator2.TryPop(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold)
   where TFold: IFold<TOut, TAccumulator> {
      if (OnIterator1) {
         (seed, OnIterator1) = Iterator1.Fold((seed, false), new InterruptingFold<TOut, TAccumulator, TFold>(fold));
      }

      return OnIterator1 ? seed : Iterator2.Fold(seed, fold);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (Iterator1.TryGetCount(out var leftCount) && Iterator2.TryGetCount(out count) && count <= int.MaxValue - leftCount) {
         count += leftCount;
         return true;
      } else {
         count = default;
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, ConcatIterator<T, TIterator1, TIterator2>> Concat<T, TIterator1, TIterator2> (
      this in Contract<IIterator<T>, TIterator1> iterator1,
      in Contract<IIterator<T>, TIterator2> iterator2
   )
   where TIterator1: IIterator<T>
   where TIterator2: IIterator<T> {
      return new ConcatIterator<T, TIterator1, TIterator2>(iterator1, iterator2);
   }
}
