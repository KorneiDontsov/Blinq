using System.Collections.Generic;

namespace Blinq;

public readonly struct ComparesItemPredicate<T, TCompareCondition, TComparer>: IItemPredicate<T>
where TCompareCondition: ICompareCondition
where TComparer: IComparer<T> {
   readonly T Value;
   readonly TCompareCondition CompareCondition;
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ComparesItemPredicate (T value, TCompareCondition compareCondition, TComparer comparer) {
      Value = value;
      CompareCondition = compareCondition;
      Comparer = comparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return Blinq.Comparer.Compares(item, Value, CompareCondition, Comparer);
   }
}
