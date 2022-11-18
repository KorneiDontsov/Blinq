using Blinq.Functors;

namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TComparer, TCondition> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TComparer comparer,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      return sequence.All(new ComparesPredicate<T, TComparer, TCondition>(value, comparer));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TComparer, TCondition> (
      this in Sequence<T, TIterator> sequence,
      T value,
      ProvideComparer<T, TComparer> provideComparer,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      return sequence.AllCompares(value, provideComparer.Invoke(), tCondition);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllCompares<T, TIterator, TCondition> (
      this in Sequence<T, TIterator> sequence,
      T value,
      Type<TCondition> tCondition = default
   )
   where TIterator: IIterator<T>
   where TCondition: ICompareCondition {
      return sequence.AllCompares(value, Get<T>.Comparer.Default(), tCondition);
   }
}
