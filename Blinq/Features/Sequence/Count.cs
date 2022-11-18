using System.Numerics;

namespace Blinq;

readonly struct CountFoldFunc<T, TCount>: IFoldFunc<T, TCount> where TCount: INumberBase<TCount> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TCount accumulator) {
      checked {
         ++accumulator;
      }

      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCount Count<T, TIterator, TCount> (this in Sequence<T, TIterator> sequence, Type<TCount> tCount = default)
   where TIterator: IIterator<T>
   where TCount: INumberBase<TCount> {
      return sequence.Count switch {
         (true, var count) => TCount.CreateChecked(count),
         _ => sequence.Iterator.Fold(TCount.Zero, new CountFoldFunc<T, TCount>()),
      };
   }

   /// <summary>Returns the number of elements in a sequence.</summary>
   /// <returns>The number of elements in <paramref name="sequence" />.</returns>
   /// <exception cref="OverflowException">
   ///    The number of elements in <paramref name="sequence" /> is larger than <see cref="Int32.MaxValue" />.
   /// </exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Count<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Count(Get.Type<int>());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static long LongCount<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Count(Get.Type<long>());
   }
}
