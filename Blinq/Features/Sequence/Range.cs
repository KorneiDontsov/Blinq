using System.Numerics;

namespace Blinq;

public struct RangeIterator<T>: IIterator<T> where T: IIncrementOperators<T> {
   T Current;
   int Count;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RangeIterator (T start, int count) {
      if (count < 0) Get.Throw<ArgumentOutOfRangeException>();

      Current = start;
      Count = count;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Count > 0) {
         --Count;
         item = Current++;
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      while (Count > 0) {
         --Count;
         if (fold.Invoke(Current++, ref seed)) break;
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Count;
      return true;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, RangeIterator<T>> Range<T, TCount> (T start, int count)
   where T: IIncrementOperators<T>
   where TCount: INumberBase<TCount> {
      return new RangeIterator<T>(start, count);
   }
}
