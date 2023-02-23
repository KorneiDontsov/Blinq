namespace Blinq.Collections;

interface ISelectOutputOfMapKeysIterator<TKey, TValue>: ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, TKey>
where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static TKey ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, TKey>.Invoke (in MapEntry<TKey, TValue> entry) {
      return entry.Key;
   }
}
