using Blinq.Functors;

namespace Blinq;

public static partial class Iterator {
   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="equaler">An equality comparer to compare values.</param>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, EqualPredicate<T, TEqualer>, TIterator>> WhereEqual<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      TEqualer equaler
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return iterator.Where(new EqualPredicate<T, TEqualer>(value, equaler));
   }

   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="provideEqualer">A function that returns an equality comparer to compare values.</param>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, EqualPredicate<T, TEqualer>, TIterator>> WhereEqual<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return WhereEqual(iterator, value, provideEqualer.Invoke());
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
   public static Contract<IIterator<T>, WhereIterator<T, EqualPredicate<T, DefaultEqualer<T>>, TIterator>> WhereEqual<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value
   ) where TIterator: IIterator<T> {
      return WhereEqual(iterator, value, Get<T>.Equaler.Default());
   }
}
