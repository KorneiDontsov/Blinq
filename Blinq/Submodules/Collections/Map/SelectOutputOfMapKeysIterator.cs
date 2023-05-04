namespace Blinq.Collections;

readonly struct SelectOutputOfMapKeysIterator<TKey, TValue>: ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, TKey>
where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TKey Invoke (in MapEntry<TKey, TValue> entry) {
      return entry.Key;
   }
}
