namespace Blinq;

readonly struct TakeFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, (TAccumulator accumulator, int countLeft)>
where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TakeFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator accumulator, int countLeft) state) {
      --state.countLeft;
      return InnerFoldFunc.Invoke(item, ref state.accumulator) || state.countLeft == 0;
   }
}

public struct TakeIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   int Count;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TakeIterator (TIterator iterator, int count) {
      Iterator = iterator;
      Count = count;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      if (Count > 0) {
         (seed, Count) = Iterator.Fold((seed, Count), new TakeFoldFunc<T, TAccumulator, TFoldFunc>(func));
      }

      return seed;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, TakeIterator<T, TIterator>> Take<T, TIterator> (this in Sequence<T, TIterator> sequence, int count)
   where TIterator: IIterator<T> {
      if (count < 0) Utils.Throw<ArgumentOutOfRangeException>();

      return Sequence<T>.Create(new TakeIterator<T, TIterator>(sequence.Iterator, count), count);
   }
}
