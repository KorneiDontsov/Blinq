using System.Collections;
using System.Collections.Generic;

namespace Blinq.Functions.Iterable;

sealed class IterableAsEnumerable<T, TIterator>: IEnumerable<T> where TIterator: IIterator<T> {
   readonly IIterable<T, TIterator> Iterable;

   public IterableAsEnumerable (IIterable<T, TIterator> iterable) {
      Iterable = iterable;
   }

   /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()" />
   public IEnumerator<T> GetEnumerator () {
      return new IteratorEnumerator<T, TIterator>(Iterable.CreateIterator());
   }

   IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator();
   }
}

public static partial class Iterable {
   /// <summary>Returns <paramref name="iterable" /> as <see cref="IEnumerable{T}" />.</summary>
   public static IEnumerable<T> AsEnumerable<T, TIterator> (this IIterable<T, TIterator> iterable) where TIterator: IIterator<T> {
      return new IterableAsEnumerable<T, TIterator>(iterable);
   }
}
