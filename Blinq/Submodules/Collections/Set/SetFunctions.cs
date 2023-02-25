namespace Blinq.Collections;

public static class SetFunctions {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static SetItemMatch<T> Match<T, TEqualer> (this ref ValueSet<T, TEqualer> set, T item)
   where T: notnull
   where TEqualer: IEqualityComparer<T> {
      var impl = TableEntryMatchImpl<T>.Create(ref set.Table, item);
      return new SetItemMatch<T>(impl, item);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static SetItemMatch<T> Match<T> (this ref ValueSet<T> set, T item) where T: notnull {
      return set.Impl.Match(item);
   }
}
