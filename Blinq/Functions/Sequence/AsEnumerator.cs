using System.Collections;
using System.Collections.Generic;

namespace Blinq;

sealed class IteratorEnumerator<T, TIterator>: IEnumerator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   T Item = default!;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public IteratorEnumerator (TIterator iterator) {
      Iterator = iterator;
   }

   public T Current => Item;

   object? IEnumerator.Current => Current;

   public bool MoveNext () {
      Iterator.Fold(Option<T>.None, new PopFoldFunc<T>()).Deconstruct(out var hasValue, out Item);
      return hasValue;
   }

   public void Dispose () { }

   public void Reset () {
      throw new NotSupportedException();
   }
}

public static partial class Sequence {
   /// <summary>Returns the iterator as <see cref="System.Collections.Generic.IEnumerator{T}" />.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static IEnumerator<T> AsEnumerator<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return new IteratorEnumerator<T, TIterator>(sequence.Iterator);
   }
}
