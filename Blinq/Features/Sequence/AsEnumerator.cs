using System.Collections;

namespace Blinq;

sealed class IteratorEnumerator<T, TIterator>: IEnumerator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   public T Current { get; private set; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public IteratorEnumerator (TIterator iterator) {
      Iterator = iterator;
      Current = default!;
   }

   object? IEnumerator.Current => Current;

   public bool MoveNext () {
      var result = Sequence<T>.Pop(ref Iterator);
      Current = result.ValueOrDefault!;
      return result.HasValue;
   }

   public void Dispose () { }

   public void Reset () {
      Get.Throw<NotSupportedException>();
   }
}

public static partial class Sequence {
   /// <summary>Returns the iterator as <see cref="System.Collections.Generic.IEnumerator{T}" />.</summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static IEnumerator<T> AsEnumerator<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return new IteratorEnumerator<T, TIterator>(sequence.Iterator);
   }
}
