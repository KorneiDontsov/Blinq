using Blinq.Functors;

namespace Blinq;

public static partial class Sequence {
   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="equaler">An equality comparer to compare values.</param>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, EqualPredicate<T, TEqualer>, TIterator>> WhereEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TEqualer equaler
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return sequence.Where(new EqualPredicate<T, TEqualer>(value, equaler));
   }

   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="provideEqualer">A function that returns an equality comparer to compare values.</param>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, EqualPredicate<T, TEqualer>, TIterator>> WhereEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return WhereEqual(sequence, value, provideEqualer.Invoke());
   }

   /// <summary>
   ///    Filters a sequence of values based on equality to a specified value.
   /// </summary>
   /// <param name="value">The value to compare to.</param>
   /// <returns>
   ///    A sequence that contains elements from the input <paramref name="sequence" /> that are equal to the specified
   ///    <paramref name="value" />.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, EqualPredicate<T, DefaultEqualer<T>>, TIterator>> WhereEqual<T, TIterator> (
      this in Sequence<T, TIterator> sequence,
      T value
   ) where TIterator: IIterator<T> {
      return WhereEqual(sequence, value, Get<T>.Equaler.Default());
   }
}