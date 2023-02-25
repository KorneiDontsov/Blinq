namespace Blinq.Collections;

struct ValueTableImpl<T> {
   public TableBucket[] Buckets = Array.Empty<TableBucket>();
   public TableCell<T>[] Cells = Array.Empty<TableCell<T>>();
   public int Size;
   public int FreeCount;
   public TableIndex FreeListIndex = TableIndex.None;
   public TablePredefinedCapacity PredefinedCapacity;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTableImpl () { }

   public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Size - FreeCount; }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly TableIteratorImpl<T, TOut, TSelectOutput> Iterate<TOut, TSelectOutput> ()
   where TSelectOutput: ISelectOutputOfTableIterator<T, TOut> {
      return new(Cells, Size, FreeCount);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly TableIterator<T> Iterate () {
      return new(Iterate<T, ISelectOutputOfTableIterator<T>>());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly CollectionEnumerator<T, TableIterator<T>> GetEnumerator () {
      return new(iterator: Iterate());
   }

   public readonly void DoCopyTo<TOut, TSelectOutput> (Span<TOut> destination) where TSelectOutput: ISelectOutputOfTableIterator<T, TOut> {
      var index = 0;
      foreach (ref var cell in Cells.AsSpan(0, Size)) {
         if (cell.Previous.IsDefined) destination[index++] = TSelectOutput.Invoke(cell.Entry);
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo<TOut, TSelectOutput> (Span<TOut> destination) where TSelectOutput: ISelectOutputOfTableIterator<T, TOut> {
      if (destination.Length < Count) Get.Throw<ArgumentOutOfRangeException>();
      DoCopyTo<TOut, TSelectOutput>(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (Span<T> destination) {
      CopyTo<T, ISelectOutputOfTableIterator<T>>(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (T[] array, int arrayIndex) {
      CopyTo(array.AsSpan(arrayIndex));
   }

   public readonly TOut[] ToArray<TOut, TSelectOutput> () where TSelectOutput: ISelectOutputOfTableIterator<T, TOut> {
      var array = new TOut[Count];
      CopyTo<TOut, TSelectOutput>(array);
      return array;
   }

   public void Clear () {
      Array.Clear(Buckets);
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) Array.Clear(Cells, 0, Size);
      Size = 0;
      FreeCount = 0;
      FreeListIndex = TableIndex.None;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal readonly ref TableBucket GetBucket (int hashCode) {
      var buckets = Buckets;
      return ref buckets[(uint)hashCode % (uint)buckets.Length];
   }

   void Resize (int newCapacity) {
      var oldCells = Cells;
      var newCells = new TableCell<T>[newCapacity];

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
      FreeCount = 0;
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
      [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Buckets.Length;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set {
         if (value < Count) Get.Throw<ArgumentOutOfRangeException>();
         if (value != Buckets.Length) {
            PredefinedCapacity = TablePredefinedCapacity.None;
            Resize(value);
         }
      }
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   public void Grow () {
      var newCapacity = PredefinedCapacity.Grow(Buckets.Length);
      Resize(newCapacity);
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   void Grow (int minCapacity) {
      var newCapacity = PredefinedCapacity.Grow(Buckets.Length);
      if (newCapacity < minCapacity) newCapacity = PredefinedCapacity.Apply(minCapacity);
      Resize(newCapacity);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      if (minCapacity < 0) Get.Throw<ArgumentOutOfRangeException>();
      if (minCapacity > Buckets.Length) Grow(minCapacity);
   }
}
