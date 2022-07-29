using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct AggregateFoldFunc<T, TAccumulator>: IFoldFunc<T, TAccumulator> {
   readonly Func<TAccumulator, T, TAccumulator> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AggregateFoldFunc (Func<TAccumulator, T, TAccumulator> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      accumulator = Func(accumulator, item);
      return false;
   }
}

readonly struct AggregateToFoldFunc<T, TAccumulator>: IFoldFunc<T, ValueTuple> where TAccumulator: class {
   readonly Action<TAccumulator, T> Action;
   readonly TAccumulator Accumulator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AggregateToFoldFunc (Action<TAccumulator, T> action, TAccumulator accumulator) {
      Action = action;
      Accumulator = accumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref ValueTuple _) {
      Action(Accumulator, item);
      return false;
   }
}

public static partial class Sequence {
   /// <summary>Applies an accumulator function over a sequence.</summary>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <returns>The final accumulator value or <see cref="Option{T}.None" /> if sequence is empty.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Aggregate<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, T, T> func) where TIterator: IIterator<T> {
      var iterator = sequence.Iterator;
      return iterator.Fold(Option<T>.None, new NextFoldFunc<T>()) switch {
         (true, var first) => Option.Value(iterator.Fold(first, new AggregateFoldFunc<T, T>(func))),
         _ => Option.None,
      };
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence. The specified <paramref name="seed" /> value is used as the initial accumulator value.
   /// </summary>
   /// <param name="seed">The initial accumulator value.</param>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
   /// <returns>The final accumulator value.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TAccumulator Aggregate<T, TIterator, TAccumulator> (
      this in Sequence<T, TIterator> sequence,
      TAccumulator seed,
      Func<TAccumulator, T, TAccumulator> func
   ) where TIterator: IIterator<T> {
      return sequence.Iterator.Fold(seed, new AggregateFoldFunc<T, TAccumulator>(func));
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence and the specified <paramref name="accumulator" />.
   /// </summary>
   /// <param name="accumulator">A mutable object that accumulates the sequence.</param>
   /// <param name="action">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulator">The type of the accumulator object.</typeparam>
   /// <returns><paramref name="accumulator" /> itself.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TAccumulator AggregateTo<T, TIterator, TAccumulator> (
      this in Sequence<T, TIterator> sequence,
      TAccumulator accumulator,
      Action<TAccumulator, T> action
   )
   where TIterator: IIterator<T>
   where TAccumulator: class {
      sequence.Iterator.Fold(Option.None, new AggregateToFoldFunc<T, TAccumulator>(action, accumulator));
      return accumulator;
   }
}
