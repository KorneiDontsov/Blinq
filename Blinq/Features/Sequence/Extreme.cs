namespace Blinq;

readonly struct ExtremeFold<T, TComparer, TCondition>: IFold<T, T>
where TComparer: IComparer<T>
where TCondition: ICompareCondition {
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ExtremeFold (TComparer comparer) {
      Comparer = comparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulator) {
      if (item.Compares(accumulator, Comparer, Get<TCondition>.Type)) {
         accumulator = item;
      }

      return false;
   }
}

public static partial class Iterator {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static Option<T> Extreme<T, TIterator, TCondition, TComparer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      TComparer comparer,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      var iter = iterator.Value;
      return iter.TryPop(out var first) ? iter.Fold(first, new ExtremeFold<T, TComparer, TCondition>(comparer)) : Option.None;
   }
}
