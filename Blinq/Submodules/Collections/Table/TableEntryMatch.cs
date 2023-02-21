namespace Blinq.Collections;

ref struct TableEntryMatchImpl<TEntry, TKey, TKeyEqualer>
where TEntry: ITableEntry<TKey>
where TKey: notnull
where TKeyEqualer: IEqualityComparer<TKey> {
   public readonly ref ValueTable<TEntry, TKey, TKeyEqualer> Table;
   ref TableBucket Bucket = ref Unsafe.NullRef<TableBucket>();
   ref TableCell<TEntry> Cell = ref Unsafe.NullRef<TableCell<TEntry>>();
   readonly int HashCode;
   int CellIndex;

   public readonly bool HasEntry { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => !Unsafe.IsNullRef(ref Cell); }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TableEntryMatchImpl (ref ValueTable<TEntry, TKey, TKeyEqualer> table, TKey key) {
      Table = table;
      HashCode = table.KeyEqualer.GetHashCode(key);

      if (table.Buckets.Length is 0) return;

      Bucket = ref table.GetBucket(HashCode);

      int index = Bucket;
      var cells = table.Cells;
      var collisionCount = 0u;
      while ((uint)index < (uint)cells.Length) {
         ref var cell = ref cells[index];
         if (cell.HashCode == HashCode && table.KeyEqualer.Equals(cell.Entry.Key, key)) {
            Cell = cell;
            CellIndex = index;
            break;
         }

         index = cell.Next;

         if (++collisionCount > (uint)cells.Length) Throw.InvalidOperationException_IteratorIsNotAllowedToBeEnumeratedTwice();
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   readonly void CheckExists () {
      if (!HasEntry) Get.Throw<InvalidOperationException>();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   readonly void CheckNotExists () {
      if (HasEntry) Get.Throw<InvalidOperationException>();
   }

   public void DoAdd (TEntry entry) {
      var cells = Table.Cells;

      int index = Table.FreeListIndex;
      if ((uint)index < (uint)cells.Length) {
         Table.FreeListIndex = cells[index].Next;
         --Table.FreeSize;
      } else {
         var size = Table.Size;
         if (size == cells.Length) {
            Table.Grow();
            cells = Table.Cells;
            Bucket = ref Table.GetBucket(HashCode);
         }

         index = size;
         Table.Size = size + 1;
      }

      CellIndex = index;
      Cell = ref cells[index];
      Cell.HashCode = HashCode;
      Cell.Next = Bucket;
      Cell.Previous = TableIndex.None;
      Cell.Entry = entry;
      Bucket = index;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (TEntry entry) {
      CheckNotExists();
      DoAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (TEntry entry) {
      if (HasEntry) {
         return false;
      } else {
         DoAdd(entry);
         return true;
      }
   }

   public TEntry Entry {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get {
         CheckExists();
         return Cell.Entry;
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set {
         if (HasEntry) {
            Cell.Entry = value;
         } else {
            DoAdd(value);
         }
      }
   }

   public void DoRemove () {
      var cells = Table.Cells;

      int previousCellIndex = Cell.Previous;
      int nextCellIndex = Cell.Next;

      if ((uint)previousCellIndex < (uint)cells.Length) {
         cells[previousCellIndex].Next = nextCellIndex;
      } else {
         Bucket = nextCellIndex;
      }

      if ((uint)nextCellIndex < (uint)cells.Length) cells[nextCellIndex].Previous = previousCellIndex;

      Cell.Next = Table.FreeListIndex;
      Cell.Previous = TableIndex.Undefined;
      if (RuntimeHelpers.IsReferenceOrContainsReferences<TEntry>()) Cell.Entry = default!;
      Table.FreeListIndex = CellIndex;

      Cell = Unsafe.NullRef<TableCell<TEntry>>();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryRemove () {
      if (HasEntry) {
         DoRemove();
         return true;
      } else {
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Remove () {
      CheckExists();
      DoRemove();
   }
}

public ref struct TableEntryMatch<TEntry, TKey, TKeyEqualer>
where TEntry: ITableEntry<TKey>
where TKey: notnull
where TKeyEqualer: IEqualityComparer<TKey> {
   TableEntryMatchImpl<TEntry, TKey, TKeyEqualer> Impl;
   public TKey Key { get; }

   public readonly bool HasEntry { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.HasEntry; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal TableEntryMatch (ref ValueTable<TEntry, TKey, TKeyEqualer> table, TKey key) {
      Impl = new(ref table, key);
      Key = key;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   readonly void CheckEntry (TEntry entry) {
      if (!Impl.Table.KeyEqualer.Equals(Key, entry.Key)) Get.Throw<ArgumentException>();
   }

   public TEntry Entry {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Impl.Entry;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set {
         CheckEntry(value);
         Impl.Entry = value;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (TEntry entry) {
      CheckEntry(entry);
      return Impl.TryAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (TEntry entry) {
      CheckEntry(entry);
      Impl.Add(entry);
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

public static class TableEntryMatch {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal static TableEntryMatchImpl<TEntry, TKey, TKeyEqualer> MatchImpl<TEntry, TKey, TKeyEqualer> (
      this ref ValueTable<TEntry, TKey, TKeyEqualer> table,
      TKey key
   )
   where TEntry: ITableEntry<TKey>
   where TKey: notnull
   where TKeyEqualer: IEqualityComparer<TKey> {
      return new TableEntryMatchImpl<TEntry, TKey, TKeyEqualer>(ref table, key);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TableEntryMatch<TEntry, TKey, TKeyEqualer> Match<TEntry, TKey, TKeyEqualer> (
      this ref ValueTable<TEntry, TKey, TKeyEqualer> table,
      TKey key
   )
   where TEntry: ITableEntry<TKey>
   where TKey: notnull
   where TKeyEqualer: IEqualityComparer<TKey> {
      return new TableEntryMatch<TEntry, TKey, TKeyEqualer>(ref table, key);
   }
}
