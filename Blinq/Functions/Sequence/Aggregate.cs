using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct AggregateAccumulator<T, TAccumulated>: IAccumulator<T, TAccumulated> {
   readonly Func<TAccumulated, T, TAccumulated> Func;

   public AggregateAccumulator (Func<TAccumulated, T, TAccumulated> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulated accumulated) {
      accumulated = Func(accumulated, item);
      return false;
   }
}

readonly struct AggregateToAccumulator<T, TAccumulated>: IAccumulator<T, ValueTuple> where TAccumulated: class {
   readonly Action<TAccumulated, T> Action;
   readonly TAccumulated Accumulator;

   public AggregateToAccumulator (Action<TAccumulated, T> action, TAccumulated accumulator) {
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
      return iterator.Accumulate(new NextAccumulator<T>(), Option<T>.None) switch {
         (true, var first) => Option.Value(iterator.Accumulate(new AggregateAccumulator<T, T>(func), first)),
         _ => Option.None,
      };
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence. The specified <paramref name="seed" /> value is used as the initial accumulator value.
   /// </summary>
   /// <param name="seed">The initial accumulator value.</param>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulated">The type of the accumulator value.</typeparam>
   /// <returns>The final accumulator value.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TAccumulated Aggregate<T, TIterator, TAccumulated> (
      this in Sequence<T, TIterator> sequence,
      TAccumulated seed,
      Func<TAccumulated, T, TAccumulated> func
   ) where TIterator: IIterator<T> {
      return sequence.Iterator.Accumulate(new AggregateAccumulator<T, TAccumulated>(func), seed);
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
      sequence.Iterator.Accumulate(new AggregateToAccumulator<T, TAccumulator>(action, accumulator), Option.None);
      return accumulator;
   }
}
