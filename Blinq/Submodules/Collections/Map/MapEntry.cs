namespace Blinq.Collections;

struct MapEntry<TKey, TValue>: ITableKeySelector<MapEntry<TKey, TValue>, TKey> where TKey: notnull {
   public TKey Key;
   public TValue Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TKey SelectKey (in MapEntry<TKey, TValue> entry) {
      return entry.Key;
   }
}
