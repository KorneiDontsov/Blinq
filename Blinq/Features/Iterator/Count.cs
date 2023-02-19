using System.Numerics;

namespace Blinq;

readonly struct CountFold<T, TCount>: IFold<T, TCount> where TCount: INumberBase<TCount> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TCount accumulator) {
      checked {
         ++accumulator;
      }

      return false;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TCount Count<T, TIterator, TCount> (this in Contract<IIterator<T>, TIterator> iterator, Type<TCount> tCount = default)
   where TIterator: IIterator<T>
   where TCount: INumberBase<TCount> {
      _ = tCount;
      var iter = iterator.Value;
      return iter.TryGetCount(out var count)
         ? TCount.CreateChecked(count)
         : iter.Fold(TCount.Zero, new CountFold<T, TCount>());
   }

   /// <summary>Returns the number of elements in a sequence.</summary>
   /// <returns>The number of elements in <paramref name="iterator" />.</returns>
   /// <exception cref="OverflowException">
   ///    The number of elements in <paramref name="iterator" /> is larger than <see cref="Int32.MaxValue" />.
   /// </exception>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Count<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return iterator.Count(Get<int>.Type);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static long LongCount<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return iterator.Count(Get<long>.Type);
   }
}
