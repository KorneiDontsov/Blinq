namespace Blinq;

readonly struct AnyFold<T>: IFold<T, bool> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   [SuppressMessage("ReSharper", "RedundantAssignment")]
   public bool Invoke (T item, ref bool accumulator) {
      accumulator = true;
      return true;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Any<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return iterator.Value.Fold(false, new AnyFold<T>());
   }
}
