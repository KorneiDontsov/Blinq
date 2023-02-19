using Blinq.Functors;

namespace Blinq;

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, ComparesPredicate<T, TComparer, TCondition>, TIterator>>
      WhereCompares<T, TIterator, TCondition, TComparer> (
         this in Contract<IIterator<T>, TIterator> iterator,
         T value,
         TComparer comparer,
         Type<TCondition> tCondition = default
      )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      return iterator.Where(new ComparesPredicate<T, TComparer, TCondition>(value, comparer));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, ComparesPredicate<T, TComparer, TCondition>, TIterator>>
      WhereCompares<T, TIterator, TComparer, TCondition> (
         this in Contract<IIterator<T>, TIterator> iterator,
         T value,
         ProvideComparer<T, TComparer> provideComparer,
         Type<TCondition> tCondition = default
      )
   where TIterator: IIterator<T>
   where TCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return iterator.WhereCompares(value, provideComparer.Invoke(), tCondition);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, ComparesPredicate<T, DefaultComparer<T>, TCondition>, TIterator>>
      WhereCompares<T, TIterator, TCondition> (
         this in Contract<IIterator<T>, TIterator> iterator,
         T value,
         Type<TCondition> tCondition = default
      )
   where TIterator: IIterator<T>
   where TCondition: ICompareCondition {
      return iterator.WhereCompares(value, Get<T>.Comparer.Default(), tCondition);
   }
}
