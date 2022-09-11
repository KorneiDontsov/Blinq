namespace Blinq;

public struct RepeatIterator<T>: IIterator<T> {
   readonly T Item;
   int Count;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RepeatIterator (T item, int count) {
      Item = item;
      Count = count;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      while (Count > 0) {
         --Count;
         if (func.Invoke(Item, ref seed)) break;
      }

      return seed;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, RepeatIterator<T>> Repeat<T> (T item, int count) {
      if (count < 0) Utils.Throw<ArgumentOutOfRangeException>();
      return Sequence<T>.Create(new RepeatIterator<T>(item, count), count);
   }
}
