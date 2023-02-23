namespace Blinq.Collections;

public ref struct MapEntryMatch<TKey, TValue> where TKey: notnull {
   TableEntryMatchImpl<MapEntry<TKey, TValue>> Impl;
   public TKey Key { get; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal MapEntryMatch (TableEntryMatchImpl<MapEntry<TKey, TValue>> impl, TKey key) {
      Impl = impl;
      Key = key;
   }

   public readonly bool HasEntry { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.HasEntry; }
   public readonly TKey EntryKey { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.EntryRef.Key; }
   public readonly ref TValue EntryValue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref Impl.EntryRef.Value; }

   internal void DoAdd (TValue value) {
      Impl.DoAdd(new() { Key = Key, Value = value });
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (TValue value) {
      Impl.CheckNotExists();
      DoAdd(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (TValue value) {
      if (Impl.HasEntry) {
         return false;
      } else {
         DoAdd(value);
         return true;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddOrReplace (TValue value) {
      if (Impl.HasEntry) {
         Impl.EntryRef.Value = value;
      } else {
         DoAdd(value);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal void DoRemove () {
      Impl.DoRemove();
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
