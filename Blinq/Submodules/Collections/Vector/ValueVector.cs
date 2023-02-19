using System.Collections;
using System.Runtime.InteropServices;
using Blinq.Functors;

namespace Blinq.Collections;

[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("ReSharper", "ConvertToAutoPropertyWithPrivateSetter")]
public struct ValueVector<T>: IReadOnlyCollection<T>, ICollection<T>, IMutVectorInternal<T> {
   const int DefaultCapacity = 4;

   T[] Items;
   int Size;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueVector () {
      Items = Array.Empty<T>();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueVector (int capacity) {
      Items = capacity > 0 ? new T[capacity] : Array.Empty<T>();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal ValueVector (T[] array) {
      Items = array;
      Size = array.Length;
   }

   [Pure] public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Size; }

   readonly bool ICollection<T>.IsReadOnly => false;

   public int Capacity {
      [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Items.Length;
      set {
         if (value < Size) Get.Throw<ArgumentOutOfRangeException>();

         if (value != Items.Length) {
            if (value > 0) {
               var newItems = new T[value];
               if (Size > 0) Array.Copy(Items, newItems, Size);
               Items = newItems;
            } else {
               Items = Array.Empty<T>();
            }
         }
      }
   }

   [Pure]
   public ref T this [int index] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get {
         if ((uint)index > Size) Get.Throw<IndexOutOfRangeException>();
         return ref Items[index];
      }
   }

   [Pure] public ref T this [Index index] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref this[index.GetOffset(Size)]; }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly T At (int index) {
      if ((uint)index > Size) Get.Throw<IndexOutOfRangeException>();
      return Items[index];
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly T At (Index index) {
      return At(index.GetOffset(Size));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly ReadOnlySpan<T> AsReadOnlySpan () {
      return new(Items, 0, Size);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<T> AsSpan () {
      return new(Items, 0, Size);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly VectorIterator<T> Iter () {
      return new(Items, Size);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly VectorEnumerator<T> GetEnumerator () {
      return new(Items, Size);
   }

   readonly IEnumerator<T> IEnumerable<T>.GetEnumerator () {
      return LightEnumeratorWrap<T>.Create(GetEnumerator());
   }

   readonly IEnumerator IEnumerable.GetEnumerator () {
      return LightEnumeratorWrap<T>.Create(GetEnumerator());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (T[] array, int arrayIndex) {
      Array.Copy(Items, 0, array, arrayIndex, Size);
   }

   [Pure]
   public readonly T[] ToArray () {
      var items = Items;
      var size = Size;
      if (size > 0) {
         var array = new T[size];
         Array.Copy(items, array, size);
         return array;
      } else {
         return Array.Empty<T>();
      }
   }

   internal T[] MoveToArray () {
      var items = Items;
      if (Size != items.Length) items = ToArray();
      this = new();
      return items;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly ValueVector<T> Copy () {
      return new(ToArray());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly ImmVector<T> ToImmutable () {
      return ImmVector<T>.Create(Copy());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly MutVector<T> ToMutable () {
      return new(Copy());
   }

   [Pure]
   public readonly int IndexOf<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      var index = 0;
      foreach (var item in Items.AsSpan(0, Size)) {
         if (predicate.Invoke(item)) return index;
         ++index;
      }

      return -1;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int IndexOf (Func<T, bool> predicate) {
      return IndexOf(new FuncPredicate<T>(predicate));
   }

   [Pure]
   public readonly int IndexOf<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return IndexOf(new EqualPredicate<T, TEqualer>(item, equaler));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int IndexOf<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return IndexOf(item, provideEqualer.Invoke());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int IndexOf (T item) {
      return Array.IndexOf(Items, item, 0, Size);
   }

   [Pure]
   public readonly int LastIndexOf<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      var index = 0;
      foreach (var item in Items.AsSpan(0, Size)) {
         if (predicate.Invoke(item)) return index;
         ++index;
      }

      return -1;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int LastIndexOf (Func<T, bool> predicate) {
      return LastIndexOf(new FuncPredicate<T>(predicate));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int LastIndexOf<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return LastIndexOf(new EqualPredicate<T, TEqualer>(item, equaler));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int LastIndexOf<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return LastIndexOf(item, provideEqualer.Invoke());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly int LastIndexOf (T item) {
      return Array.LastIndexOf(Items, item, 0, Size);
   }

   [Pure]
   public readonly bool Contains<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      foreach (var item in Items.AsSpan(0, Size)) {
         if (predicate.Invoke(item)) return true;
      }

      return false;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Contains (Func<T, bool> predicate) {
      return Contains(new FuncPredicate<T>(predicate));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Contains<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return Contains(new EqualPredicate<T, TEqualer>(item, equaler));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Contains<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Contains(item, provideEqualer.Invoke());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly bool Contains (T item) {
      return IndexOf(item) >= 0;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal readonly bool HasSame (T[] items) {
      return ReferenceEquals(Items, items);
   }

   readonly bool IMutVectorInternal<T>.HasSame (T[] items) {
      return HasSame(items);
   }

   void Grow (int capacity) {
      var newCapacity = Items.Length == 0 ? DefaultCapacity : 2 * Items.Length;
      if ((uint)newCapacity > int.MaxValue) newCapacity = int.MaxValue;
      if (newCapacity < capacity) newCapacity = capacity;
      Capacity = newCapacity;
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   void AddWithResize (T item) {
      var size = Size;
      Grow(size + 1);
      Size = size + 1;
      Items[size] = item;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      var items = Items;
      var size = Size;
      if (size < items.Length) {
         Size = size + 1;
         items[size] = item;
      } else {
         AddWithResize(item);
      }
   }

   public void AddRange (ICollection<T> collection) {
      var count = collection.Count;
      if (count > 0) {
         if (Items.Length - Size < count) Grow(checked(Size + count));
         collection.CopyTo(Items, Size);
         Size += count;
      }
   }

   void AddRangeWithPooling (IEnumerator<T> enumerator) {
      var vector = new PoolingVector<T>();
      while (enumerator.MoveNext()) vector.Add(enumerator.Current);

      var size = Size;
      Grow(size + vector.Count);
      vector.CopyTo(Items, size);
   }

   public void AddRange (IEnumerator<T> enumerator) {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         while (enumerator.MoveNext()) Add(enumerator.Current);
      } else {
         foreach (ref var item in Items.AsSpan(Size)) {
            if (!enumerator.MoveNext()) return;

            item = enumerator.Current;
            ++Size;
         }

         AddRangeWithPooling(enumerator);
      }
   }

   public void AddRange (IEnumerable<T> enumerable) {
      if (enumerable is ICollection<T> collection) {
         AddRange(collection);
      } else {
         using var enumerator = enumerable.GetEnumerator();
         AddRange(enumerator);
      }
   }

   public void Insert (int index, T item) {
      if ((uint)index > (uint)Size) Get.Throw<ArgumentOutOfRangeException>();
      if (Size == Items.Length) Grow(Size + 1);
      if (index < Size) Array.Copy(Items, index, Items, index + 1, Size - index);
      Items[index] = item;
      ++Size;
   }

   public void Insert (Index index, T item) {
      Insert(index.GetOffset(Size), item);
   }

   public void InsertRange (int index, ICollection<T> collection) {
      var count = collection.Count;
      if (count > 0) {
         if (Items.Length - Size < count) Grow(checked(Size + count));
         if (index < Size) Array.Copy(Items, index, Items, index + count, Size - index);

         if (collection is IMutVectorInternal<T> vector && vector.HasSame(Items)) {
            Array.Copy(Items, 0, Items, index, index);
            Array.Copy(Items, index + count, Items, index * 2, Size - index);
         } else {
            collection.CopyTo(Items, index);
         }

         Size += count;
      }
   }

   public void InsertRange (Index index, ICollection<T> collection) {
      InsertRange(index.GetOffset(Size), collection);
   }

   public void InsertRange (int index, IEnumerator<T> enumerator) {
      // TODO: Optimize (pooling and more)
      while (enumerator.MoveNext()) Insert(index++, enumerator.Current);
   }

   public void InsertRange (Index index, IEnumerator<T> enumerator) {
      InsertRange(index.GetOffset(Size), enumerator);
   }

   public void InsertRange (int index, IEnumerable<T> enumerable) {
      if ((uint)index > (uint)Size) Get.Throw<ArgumentOutOfRangeException>();

      if (enumerable is ICollection<T> collection) {
         InsertRange(index, collection);
      } else {
         using var enumerator = enumerable.GetEnumerator();
         InsertRange(index, enumerator);
      }
   }

   public void InsertRange (Index index, IEnumerable<T> enumerable) {
      InsertRange(index.GetOffset(Size), enumerable);
   }

   public void RemoveAt (int index) {
      if ((uint)index >= (uint)Size) Get.Throw<ArgumentOutOfRangeException>();

      --Size;
      if (index < Size) Array.Copy(Items, index + 1, Items, index, Size - index);

      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) Items[Size] = default!;
   }

   public void RemoveAt (Index index) {
      RemoveAt(index.GetOffset(Size));
   }

   public void RemoveRange (int index, int count) {
      if ((ulong)index + (ulong)count >= (ulong)Size) Get.Throw<ArgumentOutOfRangeException>();

      if (count > 0) {
         Size -= count;
         if (index < Size) Array.Copy(Items, index + count, Items, index, Size - index);

         if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) Array.Clear(Items, Size, count);
      }
   }

   public void RemoveRange (Range range) {
      var (index, count) = range.GetOffsetAndLength(Size);
      RemoveRange(index, count);
   }

   public bool Remove<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      var index = IndexOf(predicate);
      if (index >= 0) {
         RemoveAt(index);
         return true;
      } else {
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (Func<T, bool> predicate) {
      return Remove(new FuncPredicate<T>(predicate));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return Remove(new EqualPredicate<T, TEqualer>(item, equaler));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Remove(item, provideEqualer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T item) {
      var index = IndexOf(item);
      if (index >= 0) {
         RemoveAt(index);
         return true;
      } else {
         return false;
      }
   }

   public int RemoveAll<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      var freeIndex = 0; // the first free slot in items array

      // Find the first item which needs to be removed.
      while (freeIndex < Size && !predicate.Invoke(Items[freeIndex])) freeIndex++;
      if (freeIndex >= Size) return 0;

      var current = freeIndex + 1;
      while (current < Size) {
         // Find the first item which needs to be kept.
         while (current < Size && predicate.Invoke(Items[current])) current++;

         if (current < Size) {
            // copy item to the free slot.
            Items[freeIndex++] = Items[current++];
         }
      }

      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) Array.Clear(Items, freeIndex, Size - freeIndex);

      var result = Size - freeIndex;
      Size = freeIndex;
      return result;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int RemoveAll (Func<T, bool> predicate) {
      return RemoveAll(new FuncPredicate<T>(predicate));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int RemoveAll<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return RemoveAll(new EqualPredicate<T, TEqualer>(item, equaler));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int RemoveAll<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return RemoveAll(item, provideEqualer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int RemoveAll (T item) {
      return RemoveAll(item, Get<T>.Equaler.Default());
   }

   public void Clear () {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) Array.Clear(Items, 0, Size);
      Size = 0;
   }

   public void Reverse (int index, int count) {
      if ((ulong)index + (ulong)count >= (ulong)Size) Get.Throw<ArgumentOutOfRangeException>();

      if (count > 1) Array.Reverse(Items, index, count);
   }

   public void Reverse (Range range) {
      var (offset, length) = range.GetOffsetAndLength(Size);
      Reverse(offset, length);
   }

   public void Reverse () {
      var size = Size;
      if (size > 1) Array.Reverse(Items, 0, size);
   }

   public void TrimExcess () {
      if (Size < (int)(Items.Length * 0.9)) Capacity = Size;
   }
}
