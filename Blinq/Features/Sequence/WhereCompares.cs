using Blinq.Functors;

namespace Blinq;

public static partial class Sequence {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesPredicate<T, TComparer, TCondition>, TIterator>>
      WhereCompares<T, TIterator, TCondition, TComparer> (
         this in Sequence<T, TIterator> sequence,
         T value,
         TComparer comparer,
         Type<TCondition> tCondition = default
      )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      return sequence.Where(new ComparesPredicate<T, TComparer, TCondition>(value, comparer));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesPredicate<T, TComparer, TCondition>, TIterator>>
      WhereCompares<T, TIterator, TComparer, TCondition> (
         this in Sequence<T, TIterator> sequence,
         T value,
         ProvideComparer<T, TComparer> provideComparer,
         Type<TCondition> tCondition = default
      )
   where TIterator: IIterator<T>
   where TCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return sequence.WhereCompares(value, provideComparer.Invoke(), tCondition);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesPredicate<T, DefaultComparer<T>, TCondition>, TIterator>>
      WhereCompares<T, TIterator, TCondition> (
         this in Sequence<T, TIterator> sequence,
         T value,
         Type<TCondition> tCondition = default
      )
   where TIterator: IIterator<T>
   where TCondition: ICompareCondition {
      return sequence.WhereCompares(value, Get<T>.Comparer.Default(), tCondition);
   }
}
