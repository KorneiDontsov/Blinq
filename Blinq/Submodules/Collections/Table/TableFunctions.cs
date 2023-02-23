namespace Blinq.Collections;

public static class TableFunctions {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TableEntryMatch<T, TKey, TKeyEqualer, TKeySelector> Match<T, TKey, TKeyEqualer, TKeySelector> (
      this ref ValueTable<T, TKey, TKeyEqualer, TKeySelector> table,
      TKey key
   )
   where TKey: notnull
   where TKeySelector: ITableKeySelector<T, TKey>
   where TKeyEqualer: IEqualityComparer<TKey> {
      return new TableEntryMatch<T, TKey, TKeyEqualer, TKeySelector>(ref table, key);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TableEntryMatch<T, TKey, TKeyEqualer, T> Match<T, TKey, TKeyEqualer> (
      this ref ValueTable<T, TKey, TKeyEqualer> table,
      TKey key
   )
   where T: ITableKeySelector<T, TKey>
   where TKey: notnull
   where TKeyEqualer: IEqualityComparer<TKey> {
      return table.Impl.Match(key);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TableEntryMatch<T, TKey, DefaultEqualer<TKey>, T> Match<T, TKey> (this ref ValueTable<T, TKey> table, TKey key)
   where T: ITableKeySelector<T, TKey>
   where TKey: notnull {
      return table.Impl.Match(key);
   }
}
