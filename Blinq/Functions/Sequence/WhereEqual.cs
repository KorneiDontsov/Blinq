using System.Collections.Generic;

namespace Blinq;

struct WhereEqualFoldFunc<T, TEqualer, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, TAccumulator>
where TInnerFoldFunc: IFoldFunc<T, TAccumulator>
where TEqualer: IEqualityComparer<T> {
   readonly T Value;
   readonly TEqualer Equaler;
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereEqualFoldFunc (T value, TEqualer equaler, TInnerFoldFunc innerFoldFunc) {
      Value = value;
      Equaler = equaler;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return Equaler.Equals(Value, item) && InnerFoldFunc.Invoke(item, ref accumulator);
   }
}

public struct WhereEqualIterator<T, TEqualer, TIterator>: IIterator<T>
where TEqualer: IEqualityComparer<T>
where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly T Value;
   readonly TEqualer Equaler;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereEqualIterator (TIterator iterator, T value, TEqualer equaler) {
      Iterator = iterator;
      Value = value;
      Equaler = equaler;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return Iterator.Fold(seed, new WhereEqualFoldFunc<T, TEqualer, TAccumulator, TFoldFunc>(Value, Equaler, func));
   }
}

public static partial class Sequence {
   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="equaler">An equality comparer to compare values.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereEqualIterator<T, TEqualer, TIterator>> WhereEqual<T, TIterator, TEqualer> (
      this Sequence<T, TIterator> sequence,
      T value,
      TEqualer equaler
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return new WhereEqualIterator<T, TEqualer, TIterator>(sequence.Iterator, value, equaler);
   }

   /// <inheritdoc cref="WhereEqual{T,TIterator}(Sequence{T,TIterator},T)" />
   /// <param name="provideEqualer">A function that returns an equality comparer to compare values.</param>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereEqualIterator<T, TEqualer, TIterator>> WhereEqual<T, TIterator, TEqualer> (
      this Sequence<T, TIterator> sequence,
      T value,
      Func<EqualerProvider<T>, TEqualer> provideEqualer
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
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereEqualIterator<T, DefaultEqualer<T>, TIterator>> WhereEqual<T, TIterator> (
      this Sequence<T, TIterator> sequence,
      T value
   ) where TIterator: IIterator<T> {
      return WhereEqual(sequence, value, Equaler.Default<T>());
   }
}
