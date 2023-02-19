namespace Blinq;

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> First<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return iterator.Value.TryPop(out var item) ? item : Option.None;
   }
}
