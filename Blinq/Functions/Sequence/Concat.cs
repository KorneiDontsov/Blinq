namespace Blinq;

public struct ConcatIterator<TOut, TIterator1, TIterator2>: IIterator<TOut>
where TIterator1: IIterator<TOut>
where TIterator2: IIterator<TOut> {
   TIterator1 Iterator1;
   TIterator2 Iterator2;
   bool OnIterator1;

   public ConcatIterator (TIterator1 iterator1, TIterator2 iterator2) {
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
   public static Sequence<T, ConcatIterator<T, TIterator1, TIterator2>> Concat<T, TIterator1, TIterator2> (
      this in Sequence<T, TIterator1> sequence1,
      in Sequence<T, TIterator2> sequence2
   )
   where TIterator1: IIterator<T>
   where TIterator2: IIterator<T> {
      var count = (sequence1.Count, sequence2.Count) switch {
         ((true, var count1), (true, var count2)) when int.MaxValue - count1 >= count2 => Option.Value(count1 + count2),
         _ => Option.None,
      };
      var iterator = new ConcatIterator<T, TIterator1, TIterator2>(sequence1.Iterator, sequence2.Iterator);
      return new Sequence<T, ConcatIterator<T, TIterator1, TIterator2>>(iterator, count);
   }
}
