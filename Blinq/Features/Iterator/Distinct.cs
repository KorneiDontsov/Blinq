using Blinq.Collections;

namespace Blinq;

readonly struct DistinctFold<T, TAccumulator, TEqualer, TInnerFold>: IFold<T, (TAccumulator Accumulator, ValueSet<T, TEqualer> Set)>
where T: notnull
where TEqualer: IEqualityComparer<T>
where TInnerFold: IFold<T, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DistinctFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator Accumulator, ValueSet<T, TEqualer> Set) state) {
      return state.Set.TryAdd(item) && InnerFold.Invoke(item, ref state.Accumulator);
   }
}

public struct DistinctIterator<T, TEqualer, TIterator>: IIterator<T>
where T: notnull
where TEqualer: IEqualityComparer<T>
where TIterator: IIterator<T> {
   TIterator Iterator;
   ValueSet<T, TEqualer> Set;

   internal DistinctIterator (TIterator iterator, TEqualer equaler) {
      Iterator = iterator;
      Set = new(equaler);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      (accumulator, Set) = Iterator.Fold((seed: accumulator, Set), new DistinctFold<T, TAccumulator, TEqualer, TFold>(fold));
      return accumulator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      var result = Fold(Option<T>.None, new PopFold<T>());
      return result.Is(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = default;
      return false;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, DistinctIterator<T, TEqualer, TIterator>> Distinct<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      TEqualer equaler
   )
   where T: notnull
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return new DistinctIterator<T, TEqualer, TIterator>(iterator, equaler);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, DistinctIterator<T, TEqualer, TIterator>> Distinct<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where T: notnull
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return iterator.Distinct(provideEqualer());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, DistinctIterator<T, DefaultEqualer<T>, TIterator>> Distinct<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator
   )
   where T: notnull
   where TIterator: IIterator<T> {
      return iterator.Distinct(Get<T>.Equaler.Default());
   }
}
