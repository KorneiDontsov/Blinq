using Blinq.Functors;

namespace Blinq;

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TComparer, TCondition> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      TComparer comparer,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      return iterator.All(new ComparesPredicate<T, TComparer, TCondition>(value, comparer));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TComparer, TCondition> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      ProvideComparer<T, TComparer> provideComparer,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      return iterator.AllCompares(value, provideComparer.Invoke(), tCondition);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TCondition> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TCondition: ICompareCondition {
      return iterator.AllCompares(value, Get<T>.Comparer.Default(), tCondition);
   }
}
