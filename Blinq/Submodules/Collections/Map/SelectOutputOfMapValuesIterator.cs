namespace Blinq.Collections;

readonly struct SelectOutputOfMapValuesIterator<TKey, TValue>: ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, TValue>
where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TValue Invoke (in MapEntry<TKey, TValue> entry) {
      return entry.Value;
   }
}
