namespace Blinq;

readonly struct MinMaxFoldFunc<T, TCompareCondition, TComparer>: IFoldFunc<T, T>
where TCompareCondition: ICompareCondition
where TComparer: IComparer<T> {
   readonly TCompareCondition CompareCondition;
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public MinMaxFoldFunc (TCompareCondition compareCondition, TComparer comparer) {
      CompareCondition = compareCondition;
      Comparer = comparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulator) {
      if (item.Compares(accumulator, CompareCondition, Comparer)) {
         accumulator = item;
      }

      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static Option<T> MinMax<T, TIterator, TCompareCondition, TComparer> (
      this in Sequence<T, TIterator> sequence,
      TCompareCondition compareCondition,
      TComparer comparer
   )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      var iterator = sequence.Iterator;
      return Sequence<T>.Pop(ref iterator) switch {
         (true, var first) => iterator.Fold(first, new MinMaxFoldFunc<T, TCompareCondition, TComparer>(compareCondition, comparer)),
         _ => Option.None,
      };
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Min<T, TIterator, TComparer> (this in Sequence<T, TIterator> sequence, TComparer comparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return sequence.MinMax(CompareCondition.Less, comparer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Min<T, TIterator, TComparer> (this in Sequence<T, TIterator> sequence, ProvideComparer<T, TComparer> provideComparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return sequence.Min(provideComparer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Max<T, TIterator, TComparer> (this in Sequence<T, TIterator> sequence, TComparer comparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return sequence.MinMax(CompareCondition.Greater, comparer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Max<T, TIterator, TComparer> (this in Sequence<T, TIterator> sequence, ProvideComparer<T, TComparer> provideComparer)
   where TIterator: IIterator<T>
   where TComparer: IComparer<T> {
      return sequence.Max(provideComparer.Invoke());
   }
}
