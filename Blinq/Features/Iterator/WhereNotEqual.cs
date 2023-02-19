using Blinq.Functors;

namespace Blinq;

public static partial class Iterator {
   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="equaler">An equality comparer to compare values.</param>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, NotEqualPredicate<T, TEqualer>, TIterator>> WhereNotEqual<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      TEqualer equaler
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return iterator.Where(new NotEqualPredicate<T, TEqualer>(value, equaler));
   }

   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="provideEqualer">A function that returns an equality comparer to compare values.</param>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, NotEqualPredicate<T, TEqualer>, TIterator>> WhereNotEqual<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return WhereNotEqual(iterator, value, provideEqualer.Invoke());
   }

   /// <summary>
   ///    Filters a sequence of values based on equality to a specified value.
   /// </summary>
   /// <param name="value">The value to compare to.</param>
   /// <returns>
   ///    A sequence that contains elements from the input <paramref name="iterator" /> that are equal to the specified
   ///    <paramref name="value" />.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, NotEqualPredicate<T, DefaultEqualer<T>>, TIterator>> WhereNotEqual<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value
   ) where TIterator: IIterator<T> {
      return WhereNotEqual(iterator, value, Get<T>.Equaler.Default());
   }
}
