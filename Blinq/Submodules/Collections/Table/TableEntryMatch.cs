namespace Blinq.Collections;

public ref struct TableEntryMatch<T, TKey, TKeyEqualer, TKeySelector>
where TKey: notnull
where TKeySelector: ITableKeySelector<T, TKey>
where TKeyEqualer: IEqualityComparer<TKey> {
   TableEntryMatchImpl<T> Impl;
   readonly ref readonly TKeyEqualer KeyEqualer;
   public TKey Key { get; }

   public readonly bool EntryIsPresent { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.EntryIsPresent; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   #pragma warning disable CS8618
   internal TableEntryMatch (ref ValueTable<T, TKey, TKeyEqualer, TKeySelector> table, TKey key) {
      Impl = TableEntryMatchImpl<T>.Create(ref table, key);
      KeyEqualer = ref table.KeyEqualer;
      Key = key;
   }
   #pragma warning restore CS8618

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   readonly void CheckEntry (in T entry) {
      if (!KeyEqualer.Equals(Key, TKeySelector.SelectKey(entry))) Get.Throw<ArgumentException>();
   }

   public readonly T Entry { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.Entry; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T entry) {
      CheckEntry(entry);
      return Impl.TryAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T entry) {
      CheckEntry(entry);
      Impl.Add(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddOrReplace (T entry) {
      CheckEntry(entry);
      Impl.AddOrReplace(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryRemove () {
      return Impl.TryRemove();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Remove () {
      Impl.Remove();
   }
}
