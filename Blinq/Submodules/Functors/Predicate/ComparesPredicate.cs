namespace Blinq.Functors;

public readonly struct ComparesPredicate<T, TComparer, TCondition>: IPredicate<T>
where TComparer: IComparer<T>
where TCondition: ICompareCondition {
   readonly T Value;
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ComparesPredicate (T value, TComparer comparer) {
      Value = value;
      Comparer = comparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return item.Compares(Value, Comparer, Get<TCondition>.Type);
   }
}
