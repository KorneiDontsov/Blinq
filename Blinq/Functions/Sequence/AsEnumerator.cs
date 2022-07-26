using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Blinq;

sealed class IteratorEnumerator<T, TIterator>: IEnumerator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   Option<T> Item;

   public IteratorEnumerator (TIterator iterator) {
      Iterator = iterator;
      Item = Option.None;
   }

   public T Current => Item.ValueOrDefault!;

   object? IEnumerator.Current => Current;

   public bool MoveNext () {
      Item = Iterator.Accumulate(new NextAccumulator<T>(), Option<T>.None);
      return Item.HasValue;
   }

   public void Dispose () { }

   void IEnumerator.Reset () {
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
