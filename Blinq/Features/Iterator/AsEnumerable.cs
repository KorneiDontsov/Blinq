using System.Collections;

namespace Blinq;

sealed class IteratorAsEnumerable<T, TIterator>: IEnumerable<T> where TIterator: IIterator<T> {
   Option<TIterator> Iterator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public IteratorAsEnumerable (TIterator iterator) {
      Iterator = iterator;
   }

   public IEnumerator<T> GetEnumerator () {
      if (Iterator.Is(out var iterator)) {
         Iterator = Option.None;
         return new IteratorEnumerator<T, TIterator>(iterator);
      } else {
         throw new InvalidOperationException("Iterator cannot be enumerated twice.");
      }
   }

   IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator();
   }
}

public static partial class Iterator {
   /// <summary>
   ///    Returns a sequence as <see cref="IEnumerable{T}" />. Note that it can be enumerated only once.
   /// </summary>
   /// <returns>A <see cref="IEnumerable{T}" /> representation of a sequence that can be enumerated only once.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static IEnumerable<T> AsEnumerable<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return new IteratorAsEnumerable<T, TIterator>(iterator);
   }
}
