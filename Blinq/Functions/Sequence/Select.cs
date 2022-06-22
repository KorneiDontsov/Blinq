namespace Blinq;

public struct SelectIterator<TOut, TIn, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<TIn> {
   TInIterator InIterator;
   readonly Func<TIn, TOut> Selector;

   public SelectIterator (TInIterator inIterator, Func<TIn, TOut> selector) {
      InIterator = inIterator;
      Selector = selector;
   }

   /// <inheritdoc />
   public TOut Current {
      get {
         var value = InIterator.Current;
         return Selector(value);
      }
   }

   /// <inheritdoc />
   public bool MoveNext () {
      return InIterator.MoveNext();
   }
}

public static partial class Sequence {
   /// <summary>Projects each element of a sequence into a new form.</summary>
   /// <param name="selector">A transform function to apply to each element.</param>
   /// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
   /// <returns>A sequence whose elements are the result of invoking the transform function on each element of <paramref name="sequence" />.</returns>
   public static Sequence<TResult, SelectIterator<TResult, T, TIterator>> Select<T, TIterator, TResult> (
      this in Sequence<T, TIterator> sequence,
      Func<T, TResult> selector
   )
   where TIterator: IIterator<T> {
      return new Sequence<TResult, SelectIterator<TResult, T, TIterator>>(
         new SelectIterator<TResult, T, TIterator>(sequence.Iterator, selector),
         sequence.Count
      );
   }
}
