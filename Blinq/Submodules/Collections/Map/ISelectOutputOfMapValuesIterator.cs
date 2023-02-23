namespace Blinq.Collections;

interface ISelectOutputOfMapValuesIterator<TKey, TValue>: ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, TValue>
where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static TValue ISelectOutputOfTableIterator<MapEntry<TKey, TValue>, TValue>.Invoke (in MapEntry<TKey, TValue> entry) {
      return entry.Value;
   }
}
