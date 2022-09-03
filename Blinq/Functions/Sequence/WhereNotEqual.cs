using System.Collections.Generic;

namespace Blinq;

public static partial class Sequence {
   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="equaler">An equality comparer to compare values.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, NotEqualPredicate<T, TEqualer>, TIterator>> WhereNotEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TEqualer equaler
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return sequence.Where(new NotEqualPredicate<T, TEqualer>(value, equaler));
   }

   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="provideEqualer">A function that returns an equality comparer to compare values.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, NotEqualPredicate<T, TEqualer>, TIterator>> WhereNotEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return WhereNotEqual(sequence, value, provideEqualer.Invoke());
   }

   /// <summary>
   ///    Filters a sequence of values based on equality to a specified value.
   /// </summary>
   /// <param name="value">The value to compare to.</param>
   /// <returns>
   ///    A sequence that contains elements from the input <paramref name="sequence" /> that are equal to the specified
   ///    <paramref name="value" />.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, NotEqualPredicate<T, DefaultEqualer<T>>, TIterator>> WhereNotEqual<T, TIterator> (
      this in Sequence<T, TIterator> sequence,
      T value
   ) where TIterator: IIterator<T> {
      return WhereNotEqual(sequence, value, Equaler.Default<T>());
   }
}
