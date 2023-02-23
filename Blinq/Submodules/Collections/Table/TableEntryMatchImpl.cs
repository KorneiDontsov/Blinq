namespace Blinq.Collections;

ref struct TableEntryMatchImpl<T> {
   readonly ref ValueTableImpl<T> Table;
   ref TableBucket Bucket = ref Unsafe.NullRef<TableBucket>();
   ref TableCell<T> Cell = ref Unsafe.NullRef<TableCell<T>>();
   readonly int HashCode;
   int CellIndex;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TableEntryMatchImpl (ref ValueTableImpl<T> table, int hashCode) {
      Table = table;
      HashCode = hashCode;
   }

   public static TableEntryMatchImpl<T> Create<TKey, TKeyEqualer, TKeySelector> (
      ref ValueTable<T, TKey, TKeyEqualer, TKeySelector> table,
      TKey key
   )
   where TKey: notnull
   where TKeySelector: ITableKeySelector<T, TKey>
   where TKeyEqualer: IEqualityComparer<TKey> {
      var match = new TableEntryMatchImpl<T>(ref table.Impl, table.KeyEqualer.GetHashCode(key));

      if (match.Table.Buckets.Length is 0) return match;

      match.Bucket = ref match.Table.GetBucket(match.HashCode);

      int index = match.Bucket;
      var cells = match.Table.Cells;
      var collisionCount = 0u;
      while ((uint)index < (uint)cells.Length) {
         ref var cell = ref cells[index];
         if (match.HashCode == cell.HashCode && table.KeyEqualer.Equals(key, TKeySelector.SelectKey(cell.Entry))) {
            match.Cell = cell;
            match.CellIndex = index;
            break;
         }

         index = cell.Next;

         if (++collisionCount > (uint)cells.Length) Throw.InvalidOperationException_ConcurrentOperationsAreNotSupported();
      }

      return match;
   }

   public readonly bool HasEntry { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => !Unsafe.IsNullRef(ref Cell); }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CheckExists () {
      if (!HasEntry) Get.Throw<InvalidOperationException>();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CheckNotExists () {
      if (HasEntry) Get.Throw<InvalidOperationException>();
   }

   public readonly ref T EntryRef {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get {
         CheckExists();
         return ref Cell.Entry;
      }
   }

   public void DoAdd (T entry) {
      var cells = Table.Cells;

      int index = Table.FreeListIndex;
      if ((uint)index < (uint)cells.Length) {
         Table.FreeListIndex = cells[index].Next;
         --Table.FreeCount;
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
   public void Add (T entry) {
      CheckNotExists();
      DoAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T entry) {
      if (HasEntry) {
         return false;
      } else {
         DoAdd(entry);
         return true;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddOrReplace (T entry) {
      if (HasEntry) {
         Cell.Entry = entry;
      } else {
         DoAdd(entry);
      }
   }

   public readonly T Entry {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get {
         CheckExists();
         return Cell.Entry;
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
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) Cell.Entry = default!;
      Table.FreeListIndex = CellIndex;

      Cell = Unsafe.NullRef<TableCell<T>>();
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
