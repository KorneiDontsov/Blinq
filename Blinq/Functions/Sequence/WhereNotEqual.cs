using System.Collections.Generic;

namespace Blinq;

public readonly struct NotEqualItemPredicate<T, TEqualer>: IItemPredicate<T> where TEqualer: IEqualityComparer<T> {
   readonly T Value;
   readonly TEqualer Equaler;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NotEqualItemPredicate (T value, TEqualer equaler) {
      Value = value;
      Equaler = equaler;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return !Equaler.Equals(Value, item);
   }
}

public static partial class Sequence {
   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="equaler">An equality comparer to compare values.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, NotEqualItemPredicate<T, TEqualer>, TIterator>> WhereNotEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      TEqualer equaler
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return new WhereIterator<T, NotEqualItemPredicate<T, TEqualer>, TIterator>(
         sequence.Iterator,
         new NotEqualItemPredicate<T, TEqualer>(value, equaler)
      );
   }

   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="provideEqualer">A function that returns an equality comparer to compare values.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, NotEqualItemPredicate<T, TEqualer>, TIterator>> WhereNotEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      Func<EqualerProvider<T>, TEqualer> provideEqualer
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
   public static Sequence<T, WhereIterator<T, NotEqualItemPredicate<T, DefaultEqualer<T>>, TIterator>> WhereNotEqual<T, TIterator> (
      this in Sequence<T, TIterator> sequence,
      T value
   ) where TIterator: IIterator<T> {
      return WhereNotEqual(sequence, value, Equaler.Default<T>());
   }
}
