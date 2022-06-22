namespace Blinq;

public static partial class Sequence {
   /// <summary>Applies an accumulator function over a sequence.</summary>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <returns>The final accumulator value or <see cref="Option{T}.None" /> if sequence is empty.</returns>
   public static Option<T> Aggregate<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, T, T> func) where TIterator: IIterator<T> {
      var iterator = sequence.Iterator;
      if (!iterator.MoveNext()) {
         return Option<T>.None;
      } else {
         var accumulated = iterator.Current;
         while (iterator.MoveNext()) {
            var item = iterator.Current;
            accumulated = func(accumulated, item);
         }

         return accumulated;
      }
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence. The specified <paramref name="seed" /> value is used as the initial accumulator value.
   /// </summary>
   /// <param name="seed">The initial accumulator value.</param>
   /// <param name="func">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
   /// <returns>The final accumulator value.</returns>
   public static TAccumulate Aggregate<T, TIterator, TAccumulate> (
      this in Sequence<T, TIterator> sequence,
      TAccumulate seed,
      Func<TAccumulate, T, TAccumulate> func
   ) where TIterator: IIterator<T> {
      var iterator = sequence.Iterator;
      while (iterator.MoveNext()) {
         var item = iterator.Current;
         seed = func(seed, item);
      }

      return seed;
   }

   /// <summary>
   ///    Applies an accumulator function over a sequence and the specified <paramref name="accumulator" />.
   /// </summary>
   /// <param name="accumulator">A mutable object that accumulates the sequence.</param>
   /// <param name="action">An accumulator function to be invoked on each element.</param>
   /// <typeparam name="TAccumulate">The type of the accumulator object.</typeparam>
   /// <returns><paramref name="accumulator" /> itself.</returns>
   public static TAccumulate AggregateTo<T, TIterator, TAccumulate> (
      this in Sequence<T, TIterator> sequence,
      TAccumulate accumulator,
      Action<TAccumulate, T> action
   )
   where TIterator: IIterator<T>
   where TAccumulate: class {
      var iterator = sequence.Iterator;
      while (iterator.MoveNext()) {
         var item = iterator.Current;
         action(accumulator, item);
      }

      return accumulator;
   }
}
