using System.Collections;
using System.Collections.Generic;

namespace Blinq;

sealed class IteratorAsEnumerable<T, TIterator>: IEnumerable<T> where TIterator: IIterator<T> {
   Option<TIterator> Iterator;

   public IteratorAsEnumerable (TIterator iterator) {
      Iterator = iterator;
   }

   public IEnumerator<T> GetEnumerator () {
      if (Iterator is (true, var iterator)) {
         Iterator = Option<TIterator>.None;
         return new IteratorEnumerator<T, TIterator>(iterator);
      } else {
         throw new InvalidOperationException("Iterator cannot be enumerated twice.");
      }
   }

   IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator();
   }
}

public static partial class Sequence {
   /// <summary>
   ///    Returns a sequence as <see cref="IEnumerable{T}" />. Note that it can be enumerated only once.
   /// </summary>
   /// <returns>A <see cref="IEnumerable{T}" /> representation of a sequence that can be enumerated only once.</returns>
   public static IEnumerable<T> AsEnumerable<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return new IteratorAsEnumerable<T, TIterator>(sequence.Iterator);
   }
}
