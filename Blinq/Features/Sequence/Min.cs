namespace Blinq;

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Min<T, TIterator, TComparer> (this in Contract<IIterator<T>, TIterator> iterator, TComparer comparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return iterator.Extreme(comparer, CompareCondition.Less);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Min<T, TIterator, TComparer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      ProvideComparer<T, TComparer> provideComparer
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return iterator.Min(provideComparer.Invoke());
   }
}
