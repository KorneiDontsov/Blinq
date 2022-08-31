using System.Collections.Generic;

namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TCompareCondition, TComparer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TCompareCondition compareCondition,
      TComparer comparer
   )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return sequence.All(new ComparesItemPredicate<T, TCompareCondition, TComparer>(value, compareCondition, comparer));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TCompareCondition, TComparer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TCompareCondition compareCondition,
      Func<ComparerProvider<T>, TComparer> provideComparer
   )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return sequence.AllCompares(value, compareCondition, provideComparer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TCompareCondition> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TCompareCondition compareCondition
   )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition {
      return sequence.AllCompares(value, compareCondition, Comparer.Default<T>());
   }
}
