namespace Blinq;

public static partial class Iterator {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Pop<T, TIterator> (this ref Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      var iter = iterator.Value;
      if (iter.TryPop(out var item)) {
         iterator = iter;
         return Option.Value(item);
      } else {
         return Option.None;
      }
   }
}
