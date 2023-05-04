namespace Blinq.Collections;

readonly struct MapKeySelector<TKey, TValue>: ITableKeySelector<MapEntry<TKey, TValue>, TKey> where TKey: notnull {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TKey SelectKey (in MapEntry<TKey, TValue> entry) {
      return entry.Key;
   }
}
