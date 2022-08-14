namespace Blinq;

public struct ConcatIterator<TOut, TIterator>: IIterator<TOut>
where TIterator: IIterator<TOut> {
   TIterator Iterator1;
   TIterator Iterator2;
   byte IteratorIndex;

   public ConcatIterator (TIterator iterator1, TIterator iterator2) {
      Iterator1 = iterator1;
      Iterator2 = iterator2;
      IteratorIndex = 0;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) 
   where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      if (IteratorIndex == 0) {
         if (Fold(ref Iterator1, ref seed, ref func)) return seed;
         IteratorIndex = 1;
      }
      if (IteratorIndex == 1) {
         if (Fold(ref Iterator2, ref seed, ref func)) return seed;
         IteratorIndex = 2;
      }
      return seed;
   }
   
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static bool Fold<TAccumulator, TFoldFunc> (ref TIterator iterator, ref TAccumulator seed, ref TFoldFunc func) 
   where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      var outFoldFunc = new InterruptingFoldFunc<TOut, TAccumulator, TFoldFunc>(func);
      (seed, var interrupted) = iterator.Fold((seed, false), outFoldFunc);
      return interrupted;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, ConcatIterator<T, TIterator>> Concat<T, TIterator> (
      this in Sequence<T, TIterator> sequence1,
      in Sequence<T, TIterator> sequence2
   ) where TIterator: IIterator<T> {
      var count = (sequence1.Count, sequence2.Count) switch {
         ((true, var count1), (true, var count2)) => checked(count1 + count2),
         _ => Option<int>.None
      };
      var iterator = new ConcatIterator<T, TIterator>(sequence1.Iterator, sequence2.Iterator);
      return new Sequence<T, ConcatIterator<T, TIterator>>(iterator, count);
   }
}
