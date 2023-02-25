namespace Blinq.Collections;

interface ISelectOutputOfMapIterator<TKey, TValue>: ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, KeyValuePair<TKey, TValue>>
where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static KeyValuePair<TKey, TValue> ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, KeyValuePair<TKey, TValue>>.Invoke (
      in MapEntry<TKey, TValue> entry
   ) {
      return new(entry.Key, entry.Value);
   }
}
