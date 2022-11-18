using System.Numerics;

namespace Blinq;

public struct RangeIterator<T>: IIterator<T>
where T: INumberBase<T> {
   T Current;
   int CountLeft;
   bool Started;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RangeIterator (T start, int count) {
      if (count < 0) Get.Throw<ArgumentOutOfRangeException>();

      Current = start;
      CountLeft = count;
      Started = count == 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      if (!Started) {
         Started = true;
         if (func.Invoke(Current, ref seed)) return seed;
      }

      while (--CountLeft > 0) {
         ++Current;
         if (func.Invoke(Current, ref seed)) return seed;
      }

      return seed;
   }
}

public static partial class Sequence {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, RangeIterator<T>> Range<T, TCount> (T start, int count)
   where T: INumberBase<T>
   where TCount: INumberBase<TCount> {
      return Sequence<T>.Create(new RangeIterator<T>(start, count), count);
   }
}
