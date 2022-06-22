namespace Blinq.Functions.Iterable;

sealed class Iterable<T, TIterator>: IIterable<T, TIterator> where TIterator: IIterator<T> {
   readonly Func<Sequence<T, TIterator>> IteratorCreator;

   public Iterable (Func<Sequence<T, TIterator>> iteratorCreator) {
      IteratorCreator = iteratorCreator;
   }

   public TIterator CreateIterator () {
      return IteratorCreator();
   }
}

sealed class Iterable<T, TIterator, TCapture>: IIterable<T, TIterator> where TIterator: IIterator<T> {
   readonly TCapture Capture;
   readonly Func<TCapture, Sequence<T, TIterator>> IteratorCreator;

   public Iterable (TCapture capture, Func<TCapture, Sequence<T, TIterator>> iteratorCreator) {
      Capture = capture;
      IteratorCreator = iteratorCreator;
   }

   public TIterator CreateIterator () {
      return IteratorCreator(Capture);
   }
}

public static partial class Iterable {
   /// <summary>
   ///    Creates an iterable that invokes <paramref name="iteratorCreator" /> to expose the iterators.
   /// </summary>
   /// <param name="iteratorCreator">A function that creates an iterator to expose.</param>
   /// <typeparam name="T">The type of elements to iterate.</typeparam>
   /// <typeparam name="TIterator">The type of the iterator to expose.</typeparam>
   /// <returns>An iterable that invokes <paramref name="iteratorCreator" /> to expose the iterators.</returns>
   public static IIterable<T, TIterator> Create<T, TIterator> (Func<Sequence<T, TIterator>> iteratorCreator) where TIterator: IIterator<T> {
      return new Iterable<T, TIterator>(iteratorCreator);
   }

   /// <inheritdoc cref="Create{T,TIterator}(Func{Sequence{T,TIterator}}})" />
   /// <param name="capture">Any data to capture and use in <paramref name="iteratorCreator" />.</param>
   /// <typeparam name="TCapture">The type of the data to capture.</typeparam>
   public static IIterable<T, TIterator> Create<T, TIterator, TCapture> (TCapture capture, Func<TCapture, Sequence<T, TIterator>> iteratorCreator)
   where TIterator: IIterator<T> {
      return new Iterable<T, TIterator, TCapture>(capture, iteratorCreator);
   }
}
