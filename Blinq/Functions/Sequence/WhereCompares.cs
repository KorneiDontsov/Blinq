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
      return new WhereIterator<T, ComparesItemPredicate<T, TCompareCondition, TComparer>, TIterator>(
         sequence.Iterator,
         new ComparesItemPredicate<T, TCompareCondition, TComparer>(value, compareCondition, comparer)
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, ComparesItemPredicate<T, TCompareCondition, TComparer>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition, TComparer> (
         this in Sequence<T, TIterator> sequence,
         T value,
         TCompareCondition compareCondition,
         Func<ComparerProvider<T>, TComparer> provideComparer
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
