namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Min<T, TIterator, TComparer> (this in Sequence<T, TIterator> sequence, TComparer comparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return sequence.Extreme(comparer, CompareCondition.Less);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Min<T, TIterator, TComparer> (this in Sequence<T, TIterator> sequence, ProvideComparer<T, TComparer> provideComparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return sequence.Min(provideComparer.Invoke());
   }
}
