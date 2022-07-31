namespace Blinq;

readonly struct CountFoldFunc<T>: IFoldFunc<T, int> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref int accumulator) {
      checked {
         ++accumulator;
      }

      return false;
   }
}

public static partial class Sequence {
   /// <summary>Returns the number of elements in a sequence.</summary>
   /// <returns>The number of elements in <paramref name="sequence" />.</returns>
   /// <exception cref="OverflowException">
   ///    The number of elements in <paramref name="sequence" /> is larger than <see cref="Int32.MaxValue" />.
   /// </exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Count<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Count switch {
         (true, var count) => count,
         _ => sequence.Iterator.Fold(0, new CountFoldFunc<T>()),
      };
   }
}
