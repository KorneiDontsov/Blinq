using System.Collections.Generic;

namespace Blinq;

public readonly struct WhereComparesPredicate<T, TCompareCondition, TComparer>: IWherePredicate<T>
where TCompareCondition: ICompareCondition
where TComparer: IComparer<T> {
   readonly T Value;
   readonly TCompareCondition CompareCondition;
   readonly TComparer Comparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereComparesPredicate (T value, TCompareCondition compareCondition, TComparer comparer) {
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
   public static Sequence<T, WhereIterator<T, WhereComparesPredicate<T, TCompareCondition, TComparer>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition, TComparer> (
         this Sequence<T, TIterator> sequence,
         T value,
         TCompareCondition compareCondition,
         TComparer comparer
      )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return new WhereIterator<T, WhereComparesPredicate<T, TCompareCondition, TComparer>, TIterator>(
         sequence.Iterator,
         new WhereComparesPredicate<T, TCompareCondition, TComparer>(value, compareCondition, comparer)
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, WhereComparesPredicate<T, TCompareCondition, TComparer>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition, TComparer> (
         this Sequence<T, TIterator> sequence,
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
   public static Sequence<T, WhereIterator<T, WhereComparesPredicate<T, TCompareCondition, DefaultComparer<T>>, TIterator>>
      WhereCompares<T, TIterator, TCompareCondition> (
         this Sequence<T, TIterator> sequence,
         T value,
         TCompareCondition compareCondition
      )
   where TIterator: IIterator<T>
   where TCompareCondition: ICompareCondition {
      return sequence.WhereCompares(value, compareCondition, Comparer.Default<T>());
   }
}
