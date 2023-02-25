namespace Blinq.Collections;

interface IMapKeySelector<TKey, TValue>: ITableKeySelector<MapEntry<TKey, TValue>, TKey> where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static TKey ITableKeySelector<MapEntry<TKey, TValue>, TKey>.SelectKey (in MapEntry<TKey, TValue> entry) {
      return entry.Key;
   }
}
