namespace Blinq;

readonly struct AnyFoldFunc<T>: IFoldFunc<T, bool> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   [SuppressMessage("ReSharper", "RedundantAssignment")]
   public bool Invoke (T item, ref bool accumulator) {
      accumulator = true;
      return true;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Any<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Iterator.Fold(false, new AnyFoldFunc<T>());
   }
}
