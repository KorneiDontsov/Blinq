using System.Collections;

namespace Blinq.Collections;

public struct ValueTable<T, TKey, TKeyEqualer, TKeySelector>: IReadOnlyCollection<T>, ICollection<T>
where TKey: notnull
where TKeySelector: ITableKeySelector<T, TKey>
where TKeyEqualer: IEqualityComparer<TKey> {
   internal ValueTableImpl<T> Impl = new();
   internal readonly TKeyEqualer KeyEqualer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTable (TKeyEqualer keyEqualer) {
      KeyEqualer = keyEqualer;
   }

   public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.Count; }

   public int Capacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Impl.Capacity;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Impl.Capacity = value;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly TableIterator<T> Iterate () {
      return new(Impl.Iterate<T, ISelectOutputOfTableIterator<T>>());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly CollectionEnumerator<T, TableIterator<T>> GetEnumerator () {
      return Impl.GetEnumerator();
   }

   readonly IEnumerator<T> IEnumerable<T>.GetEnumerator () {
      return GetEnumerator().Box();
   }

   readonly IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator().Box();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (Span<T> destination) {
      Impl.CopyTo(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (T[] array, int arrayIndex) {
      Impl.CopyTo(array, arrayIndex);
   }

   [Pure]
   public bool Contains<TEqualer> (T entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<T> {
      var match = TableEntryMatchImpl<T>.Create(ref this, TKeySelector.SelectKey(entry));
      return match.EntryIsPresent && entryEqualer.Equals(entry, match.Entry);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T entry, ProvideEqualer<T, TEqualer> provideEntryEqualer) where TEqualer: IEqualityComparer<T> {
      return Contains(entry, provideEntryEqualer());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (T entry) {
      return Contains(entry, Get<T>.Equaler.Default());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T entry) {
      return TableEntryMatchImpl<T>.Create(ref this, TKeySelector.SelectKey(entry)).TryAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T entry) {
      var match = TableEntryMatchImpl<T>.Create(ref this, TKeySelector.SelectKey(entry));
      if (match.EntryIsPresent) Get.Throw<ArgumentException>();
      match.DoAdd(entry);
   }

   public bool Remove<TEqualer> (T entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<T> {
      var match = TableEntryMatchImpl<T>.Create(ref this, TKeySelector.SelectKey(entry));
      if (match.EntryIsPresent && entryEqualer.Equals(entry, match.Entry)) {
         match.DoRemove();
         return true;
      } else {
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T entry, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Remove(entry, provideEqualer());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T entry) {
      return Remove(entry, Get<T>.Equaler.Default());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear () {
      Impl.Clear();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Impl.EnsureCapacity(minCapacity);
   }

   #region ICollection
   readonly bool ICollection<T>.IsReadOnly => false;

   void ICollection<T>.Add (T item) {
      _ = TryAdd(item);
   }
   #endregion
}

public struct ValueTable<T, TKey, TKeyEqualer>: IReadOnlyCollection<T>, ICollection<T>
where T: ITableKeySelector<T, TKey>
where TKey: notnull
where TKeyEqualer: IEqualityComparer<TKey> {
   internal ValueTable<T, TKey, TKeyEqualer, T> Impl;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTable (TKeyEqualer keyEqualer) {
      Impl = new(keyEqualer);
   }

   public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.Count; }

   public int Capacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Impl.Capacity;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Impl.Capacity = value;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly TableIterator<T> Iterate () {
      return Impl.Iterate();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly CollectionEnumerator<T, TableIterator<T>> GetEnumerator () {
      return Impl.GetEnumerator();
   }

   readonly IEnumerator<T> IEnumerable<T>.GetEnumerator () {
      return GetEnumerator().Box();
   }

   readonly IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator().Box();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (Span<T> destination) {
      Impl.CopyTo(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (T[] array, int arrayIndex) {
      Impl.CopyTo(array, arrayIndex);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Contains(entry, entryEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T entry, ProvideEqualer<T, TEqualer> provideEntryEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Contains(entry, provideEntryEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (T entry) {
      return Impl.Contains(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T entry) {
      return Impl.TryAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T entry) {
      Impl.Add(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Remove(entry, entryEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T entry, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Remove(entry, provideEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T entry) {
      return Impl.Remove(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear () {
      Impl.Clear();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Impl.EnsureCapacity(minCapacity);
   }

   #region ICollection
   readonly bool ICollection<T>.IsReadOnly => false;

   void ICollection<T>.Add (T item) {
      _ = TryAdd(item);
   }
   #endregion
}

public struct ValueTable<T, TKey>: IReadOnlyCollection<T>, ICollection<T>
where T: ITableKeySelector<T, TKey>
where TKey: notnull {
   internal ValueTable<T, TKey, DefaultEqualer<TKey>, T> Impl = new(keyEqualer: new());

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueTable () { }

   public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.Count; }

   public int Capacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] readonly get => Impl.Capacity;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Impl.Capacity = value;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly TableIterator<T> Iterate () {
      return Impl.Iterate();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly CollectionEnumerator<T, TableIterator<T>> GetEnumerator () {
      return Impl.GetEnumerator();
   }

   readonly IEnumerator<T> IEnumerable<T>.GetEnumerator () {
      return GetEnumerator().Box();
   }

   readonly IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator().Box();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (Span<T> destination) {
      Impl.CopyTo(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (T[] array, int arrayIndex) {
      Impl.CopyTo(array, arrayIndex);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Contains(entry, entryEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T entry, ProvideEqualer<T, TEqualer> provideEntryEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Contains(entry, provideEntryEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (T entry) {
      return Impl.Contains(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T entry) {
      return Impl.TryAdd(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T entry) {
      Impl.Add(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T entry, TEqualer entryEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Remove(entry, entryEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T entry, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Impl.Remove(entry, provideEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T entry) {
      return Impl.Remove(entry);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear () {
      Impl.Clear();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Impl.EnsureCapacity(minCapacity);
   }

   #region ICollection
   readonly bool ICollection<T>.IsReadOnly => false;

   void ICollection<T>.Add (T item) {
      _ = TryAdd(item);
   }
   #endregion
}
