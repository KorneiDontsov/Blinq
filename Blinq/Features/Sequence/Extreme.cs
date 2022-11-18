namespace Blinq;

readonly struct ExtremeFoldFunc<T, TComparer, TCondition>: IFoldFunc<T, T>
where TComparer: IComparer<T>
where TCondition: ICompareCondition {
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ExtremeFoldFunc (TComparer comparer) {
      Comparer = comparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulator) {
      if (item.Compares(accumulator, Comparer, Get.Type<TCondition>())) {
         accumulator = item;
      }

      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static Option<T> Extreme<T, TIterator, TCondition, TComparer> (
      this in Sequence<T, TIterator> sequence,
      TComparer comparer,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      var iterator = sequence.Iterator;
      return Sequence<T>.Pop(ref iterator) switch {
         (true, var first) => iterator.Fold(first, new ExtremeFoldFunc<T, TComparer, TCondition>(comparer)),
         _ => Option.None,
      };
   }
}
