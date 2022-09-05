namespace Blinq;

public static partial class Sequence {
   /// <summary>This method is just a shortcut for <see cref="Flatten" /> over <see cref="Select" />.</summary>
   /// <param name="selector">A transform function to apply to each element.</param>
   /// <returns>
   ///    A sequence whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<
      TResult,
      FlattenIterator<
         TResult,
         TResultIterator,
         SelectIterator<Sequence<TResult, TResultIterator>, T, FuncSelector<T, Sequence<TResult, TResultIterator>>, TIterator>>
   > SelectMany<T, TIterator, TResult, TResultIterator> (
      this in Sequence<T, TIterator> sequence,
      Func<T, Sequence<TResult, TResultIterator>> selector
   )
   where TIterator: IIterator<T>
   where TResultIterator: IIterator<TResult> {
      return new FlattenIterator<
         TResult,
         TResultIterator,
         SelectIterator<Sequence<TResult, TResultIterator>, T, FuncSelector<T, Sequence<TResult, TResultIterator>>, TIterator>
      >(
         new SelectIterator<Sequence<TResult, TResultIterator>, T, FuncSelector<T, Sequence<TResult, TResultIterator>>, TIterator>(
            sequence.Iterator,
            new FuncSelector<T, Sequence<TResult, TResultIterator>>(selector)
         )
      );
   }
}
