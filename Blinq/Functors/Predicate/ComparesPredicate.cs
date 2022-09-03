using System.Collections.Generic;

namespace Blinq;

public readonly struct ComparesPredicate<T, TCompareCondition, TComparer>: IPredicate<T>
where TCompareCondition: ICompareCondition
where TComparer: IComparer<T> {
   readonly T Value;
   readonly TCompareCondition CompareCondition;
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ComparesPredicate (T value, TCompareCondition compareCondition, TComparer comparer) {
      Value = value;
      CompareCondition = compareCondition;
      Comparer = comparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return item.Compares(Value, CompareCondition, Comparer);
   }
}
