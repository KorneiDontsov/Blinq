namespace Blinq.Collections;

public static class MapFunctions {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static MapEntryMatch<TKey, TValue> Match<TKey, TValue, TKeyEqualer> (this ref ValueMap<TKey, TValue, TKeyEqualer> map, TKey key)
   where TKey: notnull
   where TKeyEqualer: IEqualityComparer<TKey> {
      var impl = TableEntryMatchImpl<MapEntry<TKey, TValue>>.Create(ref map.Table, key);
      return new MapEntryMatch<TKey, TValue>(impl, key);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static MapEntryMatch<TKey, TValue> Match<TKey, TValue> (this ref ValueMap<TKey, TValue> map, TKey key) where TKey: notnull {
      return map.Impl.Match(key);
   }
}
