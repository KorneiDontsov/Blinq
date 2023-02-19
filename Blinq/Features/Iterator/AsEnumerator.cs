using System.Collections;

namespace Blinq;

sealed class IteratorEnumerator<T, TIterator>: IEnumerator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   T? Item;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public IteratorEnumerator (TIterator iterator) {
      Iterator = iterator;
   }

   public T Current => Item!;
   object? IEnumerator.Current => Current;

   public bool MoveNext () {
      return Iterator.TryPop(out Item);
   }

   public void Dispose () { }

   public void Reset () {
      Get.Throw<NotSupportedException>();
   }
}

public static partial class Iterator {
   /// <summary>Returns the iterator as <see cref="System.Collections.Generic.IEnumerator{T}" />.</summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static IEnumerator<T> AsEnumerator<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator) where TIterator: IIterator<T> {
      return new IteratorEnumerator<T, TIterator>(iterator);
   }
}
