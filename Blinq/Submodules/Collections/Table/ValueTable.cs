using System.Collections;

namespace Blinq.Collections;

public struct ValueTable<TEntry, TKey, TKeyEqualer>: IReadOnlyCollection<TEntry>, ICollection<TEntry>
where TEntry: ITableEntry<TKey>
where TKey: notnull
where TKeyEqualer: IEqualityComparer<TKey> {
   internal TableBucket[] Buckets;
   internal TableCell<TEntry>[] Cells;
   internal int Size;
   internal int FreeSize;
   internal TableIndex FreeListIndex = TableIndex.None;
   internal TablePredefinedCapacity PredefinedCapacity;
   internal readonly TKeyEqualer KeyEqualer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTable (TKeyEqualer keyEqualer) {
      KeyEqualer = keyEqualer;
      Buckets = Array.Empty<TableBucket>();
      Cells = Array.Empty<TableCell<TEntry>>();
   }

   public readonly int Count { [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Size - FreeSize; }

   [Pure]
   public readonly TableEnumerator<TEntry> GetEnumerator () {
      return new(Cells, Size);
   }

   IEnumerator<TEntry> IEnumerable<TEntry>.GetEnumerator () {
      return LightEnumeratorWrap<TEntry>.Create(GetEnumerator());
   }

   IEnumerator IEnumerable.GetEnumerator () {
      return LightEnumeratorWrap<TEntry>.Create(GetEnumerator());
   }

   readonly void ICollection<TEntry>.CopyTo (TEntry[] array, int arrayIndex) {
      if ((uint)arrayIndex > (uint)array.Length || array.Length - arrayIndex < Count) Get.Throw<ArgumentOutOfRangeException>();

      foreach (ref var cell in Cells.AsSpan(0, Size)) {
         if (cell.Previous.IsDefined) array[arrayIndex++] = cell.Entry;
      }
   }

   [Pure]
   public bool Contains<TEqualer> (TEntry entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<TEntry> {
      var match = this.MatchImpl(entry.Key);
      return match.HasEntry && entryEqualer.Equals(entry, match.Entry);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (TEntry entry, ProvideEqualer<TEntry, TEqualer> provideEntryEqualer) where TEqualer: IEqualityComparer<TEntry> {
      return Contains(entry, provideEntryEqualer());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (TEntry entry) {
      return Contains(entry, Get<TEntry>.Equaler.Default());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (TEntry entry) {
      return this.MatchImpl(entry.Key).TryAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (TEntry entry) {
      var match = this.MatchImpl(entry.Key);
      if (match.HasEntry) Get.Throw<ArgumentException>();
      match.DoAdd(entry);
   }

   public bool Remove<TEqualer> (TEntry entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<TEntry> {
      var match = this.MatchImpl(entry.Key);
      if (match.HasEntry && entryEqualer.Equals(entry, match.Entry)) {
         match.DoRemove();
         return true;
      } else {
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (TEntry entry, ProvideEqualer<TEntry, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<TEntry> {
      return Remove(entry, provideEqualer());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (TEntry entry) {
      return Remove(entry, Get<TEntry>.Equaler.Default());
   }

   public void Clear () {
      Array.Clear(Buckets);
      if (RuntimeHelpers.IsReferenceOrContainsReferences<TEntry>()) Array.Clear(Cells, 0, Size);
      Size = 0;
      FreeSize = 0;
      FreeListIndex = TableIndex.None;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal readonly ref TableBucket GetBucket (int hashCode) {
      var buckets = Buckets;
      return ref buckets[(uint)hashCode % (uint)buckets.Length];
   }

   void Resize (int newCapacity) {
      var oldCells = Cells;
      var newCells = new TableCell<TEntry>[newCapacity];

      int count;
      var oldIndex = 0;
      var newIndex = 0;
      var freeListIndex = FreeListIndex;

      while (freeListIndex.HasValue) {
         count = freeListIndex - oldIndex;
         Array.Copy(oldCells, oldIndex, newCells, newIndex, count);

         oldIndex = freeListIndex + 1;
         newIndex += count;
         freeListIndex = oldCells[freeListIndex].Next;
      }

      count = Size - oldIndex;
      Array.Copy(oldCells, oldIndex, newCells, newIndex, count);
      var size = newIndex + count;

      Cells = newCells;
      Size = size;
      FreeSize = 0;
      FreeListIndex = TableIndex.None;

      Buckets = new TableBucket[newCapacity];

      var index = 0;
      foreach (ref var cell in newCells.AsSpan(0, size)) {
         if (cell.Previous.IsDefined) {
            ref var bucket = ref GetBucket(cell.HashCode);
            int nextIndex = bucket;
            bucket = index;
            cell.Next = nextIndex;
            cell.Previous = TableIndex.None;
            if ((uint)nextIndex < (uint)newCells.Length) newCells[nextIndex].Previous = index;
         }

         ++index;
      }
   }

   public int Capacity {
      [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Buckets.Length;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set {
         if (value < Count) Get.Throw<ArgumentOutOfRangeException>();
         if (value != Buckets.Length) {
            PredefinedCapacity = TablePredefinedCapacity.None;
            Resize(value);
         }
      }
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   internal void Grow () {
      var newCapacity = PredefinedCapacity.Grow(Buckets.Length);
      Resize(newCapacity);
   }

   #region ICollection
   readonly bool ICollection<TEntry>.IsReadOnly => false;

   void ICollection<TEntry>.Add (TEntry item) {
      _ = TryAdd(item);
   }
   #endregion
}
