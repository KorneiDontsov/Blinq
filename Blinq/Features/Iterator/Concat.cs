namespace Blinq;

public struct ConcatIterator<TOut, T1Iterator, T2Iterator>: IIterator<TOut>
where T1Iterator: IIterator<TOut>
where T2Iterator: IIterator<TOut> {
   T1Iterator Iterator1;
   T2Iterator Iterator2;
   bool OnIterator1 = true;

   public ConcatIterator (T1Iterator iterator1, T2Iterator iterator2) {
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
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold)
   where TFold: IFold<TOut, TAccumulator> {
      if (OnIterator1) {
         (accumulator, OnIterator1) = Iterator1.Fold((seed: accumulator, false), new InterruptingFold<TOut, TAccumulator, TFold>(fold));
      }

      return OnIterator1 ? accumulator : Iterator2.Fold(accumulator, fold);
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
   public static Contract<IIterator<T>, ConcatIterator<T, T1Iterator, T2Iterator>> Concat<T, T1Iterator, T2Iterator> (
      this in Contract<IIterator<T>, T1Iterator> iterator1,
      in Contract<IIterator<T>, T2Iterator> iterator2
   )
   where T1Iterator: IIterator<T>
   where T2Iterator: IIterator<T> {
      return new ConcatIterator<T, T1Iterator, T2Iterator>(iterator1, iterator2);
   }
}
