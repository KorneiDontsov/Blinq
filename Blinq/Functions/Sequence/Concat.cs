namespace Blinq;

public struct ConcatIterator<TOut, TIterator>: IIterator<TOut>
where TIterator: IIterator<TOut> {
   TIterator Iterator1;
   TIterator Iterator2;
   bool OnIterator1;

   public ConcatIterator (TIterator iterator1, TIterator iterator2) {
      Iterator1 = iterator1;
      Iterator2 = iterator2;
      OnIterator1 = true;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func)
   where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      if (OnIterator1) {
         (seed, OnIterator1) = Iterator1.Fold((seed, false), new InterruptingFoldFunc<TOut, TAccumulator, TFoldFunc>(func));
      }

      return OnIterator1 ? seed : Iterator2.Fold(seed, func);
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, ConcatIterator<T, TIterator>> Concat<T, TIterator> (
      this in Sequence<T, TIterator> sequence1,
      in Sequence<T, TIterator> sequence2
   ) where TIterator: IIterator<T> {
      var count = (sequence1.Count, sequence2.Count) switch {
         ((true, var count1), (true, var count2)) => Option.Value(checked(count1 + count2)),
         _ => Option.None,
      };
      var iterator = new ConcatIterator<T, TIterator>(sequence1.Iterator, sequence2.Iterator);
      return new Sequence<T, ConcatIterator<T, TIterator>>(iterator, count);
   }
}
