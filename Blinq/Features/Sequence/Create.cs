namespace Blinq;

public static partial class Sequence<T> {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, TIterator> Create<TIterator> (TIterator iterator, Option<int> count = default) where TIterator: IIterator<T> {
      return new Sequence<T, TIterator>(iterator, count);
   }
}
