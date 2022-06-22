namespace Blinq;

public struct WhereIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly Func<T, bool> Predicate;

   /// <inheritdoc />
   public T Current { get; private set; }

   public WhereIterator (TIterator iterator, Func<T, bool> predicate) {
      Iterator = iterator;
      Predicate = predicate;
      Current = default!;
   }

   /// <inheritdoc />
   public bool MoveNext () {
      while (Iterator.MoveNext()) {
         Current = Iterator.Current;
         if (Predicate(Current)) return true;
      }

      return false;
   }
}

public static partial class Sequence {
   /// <summary>Filters a sequence of values based on a predicate.</summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <returns>A sequence that contains elements from the input sequence that satisfy the condition.</returns>
   public static Sequence<T, WhereIterator<T, TIterator>> Where<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, bool> predicate)
   where TIterator: IIterator<T> {
      return new WhereIterator<T, TIterator>(sequence.Iterator, predicate);
   }
}
