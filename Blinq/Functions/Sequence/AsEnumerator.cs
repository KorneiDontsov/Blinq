using System.Collections;
using System.Collections.Generic;

namespace Blinq;

sealed class IteratorEnumerator<T, TIterator>: IEnumerator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   public T Current { get; private set; }

   public IteratorEnumerator (TIterator iterator) {
      Iterator = iterator;
      Current = default!;
   }

   object? IEnumerator.Current => Current;

   public bool MoveNext () {
      if (Iterator.MoveNext()) {
         Current = Iterator.Current;
         return true;
      } else {
         return false;
      }
   }

   public void Dispose () { }

   void IEnumerator.Reset () {
      throw new NotSupportedException();
   }
}

public static partial class Sequence {
   /// <summary>Returns the iterator as <see cref="System.Collections.Generic.IEnumerator{T}" />.</summary>
   public static IEnumerator<T> AsEnumerator<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return new IteratorEnumerator<T, TIterator>(sequence.Iterator);
   }
}
