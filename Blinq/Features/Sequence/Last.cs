namespace Blinq;

readonly struct LastFold<T>: IFold<T, Option<T>> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   [SuppressMessage("ReSharper", "RedundantAssignment")]
   public bool Invoke (T item, ref Option<T> accumulator) {
      accumulator = Option.Value(item);
      return false;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Last<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return iterator.Value.Fold(Option<T>.None, new LastFold<T>());
   }
}
