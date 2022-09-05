namespace Blinq;

readonly struct LastFoldFunc<T>: IFoldFunc<T, Option<T>> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   [SuppressMessage("ReSharper", "RedundantAssignment")]
   public bool Invoke (T item, ref Option<T> accumulator) {
      accumulator = Option.Value(item);
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Last<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Iterator.Fold(Option<T>.None, new LastFoldFunc<T>());
   }
}
