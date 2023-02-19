using Blinq.Functors;

namespace Blinq;

public static partial class Iterator {
   /// <summary>This method is just a shortcut for <see cref="Flatten" /> over <see cref="Select" />.</summary>
   /// <param name="selector">A transform function to apply to each element.</param>
   /// <returns>
   ///    A sequence whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<
      IIterator<TResult>,
      FlattenIterator<
         TResult,
         TResultIterator,
         SelectIterator<Contract<IIterator<TResult>, TResultIterator>, T, FuncSelector<T, Contract<IIterator<TResult>, TResultIterator>>, TIterator>>
   > SelectMany<T, TIterator, TResult, TResultIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Func<T, Contract<IIterator<TResult>, TResultIterator>> selector
   )
   where TIterator: IIterator<T>
   where TResultIterator: IIterator<TResult> {
      return new FlattenIterator<
         TResult,
         TResultIterator,
         SelectIterator<Contract<IIterator<TResult>, TResultIterator>, T, FuncSelector<T, Contract<IIterator<TResult>, TResultIterator>>, TIterator>
      >(
         new SelectIterator<Contract<IIterator<TResult>, TResultIterator>, T, FuncSelector<T, Contract<IIterator<TResult>, TResultIterator>>,
            TIterator>(
            iterator,
            new FuncSelector<T, Contract<IIterator<TResult>, TResultIterator>>(selector)
         )
      );
   }
}
