namespace Blinq.Collections;

readonly struct SelectOutputOfMapIterator<TKey, TValue>: ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, KeyValuePair<TKey, TValue>>
where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static KeyValuePair<TKey, TValue> Invoke (in MapEntry<TKey, TValue> entry) {
      return new(entry.Key, entry.Value);
   }
}
