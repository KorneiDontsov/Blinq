namespace Blinq.Collections;

public struct CollectionEnumerator<T, TIterator>: ICollectionEnumerator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   T? Item;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal CollectionEnumerator (TIterator iterator) {
      Iterator = iterator;
   }

   public readonly T Current { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Item!; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool MoveNext () {
      return Iterator.TryPop(out Item);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal CollectionEnumeratorBox<T, CollectionEnumerator<T, TIterator>> Box () {
      return new(enumerator: this);
   }
}
