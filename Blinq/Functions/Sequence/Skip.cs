namespace Blinq;

readonly struct SkipFoldFunc<T>: IFoldFunc<T, int> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref int accumulator) {
      return --accumulator == 0;
   }
}

public struct SkipIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly int Count;
   bool Skipped;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SkipIterator (TIterator iterator, int count) {
      Iterator = iterator;
      Count = count;
      Skipped = Count == 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      if (!Skipped) {
         Iterator.Fold(Count, new SkipFoldFunc<T>());
         Skipped = true;
      }

      return Iterator.Fold(seed, func);
   }
}

public static partial class Sequence {
   public static Sequence<T, SkipIterator<T, TIterator>> Skip<T, TIterator> (this in Sequence<T, TIterator> sequence, int count)
   where TIterator: IIterator<T> {
      if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), count, null);

      var newCount = sequence.Count switch {
         (true, var beginCount) => Option.Value(System.Math.Max(0, beginCount - count)),
         _ => Option.None,
      };
      return Sequence<T>.Create(new SkipIterator<T, TIterator>(sequence.Iterator, count), newCount);
   }
}
