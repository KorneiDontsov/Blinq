namespace Blinq;

readonly struct AggregateFold<T, TAccumulator>: IFold<T, TAccumulator> {
   readonly Func<TAccumulator, T, TAccumulator> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AggregateFold (Func<TAccumulator, T, TAccumulator> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      accumulator = Func(accumulator, item);
      return false;
   }
}

readonly struct AggregateToFold<T, TAccumulator>: IFold<T, ValueTuple> where TAccumulator: class {
   readonly Action<TAccumulator, T> Action;
   readonly TAccumulator Accumulator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AggregateToFold (Action<TAccumulator, T> action, TAccumulator accumulator) {
      Action = action;
      Accumulator = accumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref ValueTuple _) {
      Action(Accumulator, item);
      return false;
   }
}

public static partial class Iterator {
   /// <summary>Applies an accumulator function over a sequence.</summary>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <returns>The final accumulator value or <see cref="Option{T}.None" /> if sequence is empty.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Aggregate<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, Func<T, T, T> func)
   where TIterator: IIterator<T> {
      var iter = iterator.Value;
      return iter.TryPop(out var first) ? Option.Value(iter.Fold(first, new AggregateFold<T, T>(func))) : Option.None;
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence. The specified <paramref name="seed" /> value is used as the initial accumulator value.
   /// </summary>
   /// <param name="seed">The initial accumulator value.</param>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
   /// <returns>The final accumulator value.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TAccumulator Aggregate<T, TIterator, TAccumulator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      TAccumulator seed,
      Func<TAccumulator, T, TAccumulator> func
   ) where TIterator: IIterator<T> {
      return iterator.Value.Fold(seed, new AggregateFold<T, TAccumulator>(func));
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence and the specified <paramref name="accumulator" />.
   /// </summary>
   /// <param name="accumulator">A mutable object that accumulates the sequence.</param>
   /// <param name="action">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulator">The type of the accumulator object.</typeparam>
   /// <returns><paramref name="accumulator" /> itself.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TAccumulator AggregateTo<T, TIterator, TAccumulator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      TAccumulator accumulator,
      Action<TAccumulator, T> action
   )
   where TIterator: IIterator<T>
   where TAccumulator: class {
      iterator.Value.Fold(Option.None, new AggregateToFold<T, TAccumulator>(action, accumulator));
      return accumulator;
   }
}
