using System.Collections.Generic;

namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesItemPredicate<T, TCompareCondition, TComparer>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition, TComparer> (
         this in Sequence<T, TIterator> sequence,
         T value,
         TCompareCondition compareCondition,
         TComparer comparer
      )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return sequence.Where(new ComparesItemPredicate<T, TCompareCondition, TComparer>(value, compareCondition, comparer));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesItemPredicate<T, TCompareCondition, TComparer>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition, TComparer> (
         this in Sequence<T, TIterator> sequence,
         T value,
         TCompareCondition compareCondition,
         ProvideComparer<T, TComparer> provideComparer
      )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return sequence.WhereCompares(value, compareCondition, provideComparer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesItemPredicate<T, TCompareCondition, DefaultComparer<T>>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition> (
         this in Sequence<T, TIterator> sequence,
         T value,
         TCompareCondition compareCondition
      )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition {
      return sequence.WhereCompares(value, compareCondition, Comparer.Default<T>());
   }
}
