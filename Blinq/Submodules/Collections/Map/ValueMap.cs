using System.Collections;

namespace Blinq.Collections;

public struct ValueMap<TKey, TValue, TKeyEqualer>: IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>
where TKey: notnull
where TKeyEqualer: IEqualityComparer<TKey> {
   internal ValueTable<MapEntry<TKey, TValue>, TKey, TKeyEqualer, MapKeySelector<TKey, TValue>> Table;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueMap (TKeyEqualer keyEqualer) {
      Table = new(keyEqualer);
   }

   public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Table.Count; }

   public TValue this [TKey key] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get {
         var match = this.Match(key);
         if (!match.EntryIsPresent) Get.Throw<KeyNotFoundException>();
         return match.EntryValue;
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set => this.Match(key).AddOrReplace(value);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly MapIterator<TKey, TValue> Iterate () {
      return new(Table.Impl.Iterate<KeyValuePair<TKey, TValue>, SelectOutputOfMapIterator<TKey, TValue>>());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly CollectionEnumerator<KeyValuePair<TKey, TValue>, MapIterator<TKey, TValue>> GetEnumerator () {
      return new(iterator: Iterate());
   }

   readonly IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator () {
      return GetEnumerator().Box();
   }

   readonly IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator().Box();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool ContainsKey (TKey key) {
      return this.Match(key).EntryIsPresent;
   }

   [Pure]
   public bool Contains<TValueEqualer> (TKey key, TValue value, TValueEqualer valueEqualer) where TValueEqualer: IEqualityComparer<TValue> {
      var match = this.Match(key);
      return match.EntryIsPresent && valueEqualer.Equals(value, match.EntryValue);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TValueEqualer> (TKey key, TValue value, ProvideEqualer<TValue, TValueEqualer> provideValueEqualer)
   where TValueEqualer: IEqualityComparer<TValue> {
      return Contains(key, value, provideValueEqualer());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (TKey key, TValue value) {
      return Contains(key, value, Get<TValue>.Equaler.Default());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetValue (TKey key, [MaybeNullWhen(false)] out TValue value) {
      var match = this.Match(key);
      if (match.EntryIsPresent) {
         value = match.EntryValue;
         return true;
      } else {
         value = default;
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (Span<KeyValuePair<TKey, TValue>> destination) {
      Table.Impl.CopyTo<KeyValuePair<TKey, TValue>, SelectOutputOfMapIterator<TKey, TValue>>(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      CopyTo(array.AsSpan(arrayIndex));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal readonly TKey[] CopyKeys () {
      return Table.Impl.ToArray<TKey, SelectOutputOfMapKeysIterator<TKey, TValue>>();
   }

   readonly IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => CopyKeys(); }
   readonly ICollection<TKey> IDictionary<TKey, TValue>.Keys { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => CopyKeys(); }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal readonly TValue[] CopyValues () {
      return Table.Impl.ToArray<TValue, SelectOutputOfMapValuesIterator<TKey, TValue>>();
   }

   readonly IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => CopyValues(); }
   readonly ICollection<TValue> IDictionary<TKey, TValue>.Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => CopyValues(); }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (TKey key, TValue value) {
      var match = this.Match(key);
      if (match.EntryIsPresent) Get.Throw<ArgumentException>();
      match.DoAdd(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (TKey key, TValue value) {
      return this.Match(key).TryAdd(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (TKey key) {
      return this.Match(key).TryRemove();
   }

   public bool Remove<TValueEqualer> (TKey key, TValue value, TValueEqualer valueEqualer) where TValueEqualer: IEqualityComparer<TValue> {
      var match = this.Match(key);
      if (match.EntryIsPresent && valueEqualer.Equals(value, match.EntryValue)) {
         match.DoRemove();
         return true;
      } else {
         return false;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TValueEqualer> (TKey key, TValue value, ProvideEqualer<TValue, TValueEqualer> provideValueEqualer)
   where TValueEqualer: IEqualityComparer<TValue> {
      return Remove(key, value, provideValueEqualer());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (TKey key, TValue value) {
      return Remove(key, value, Get<TValue>.Equaler.Default());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear () {
      Table.Clear();
   }

   #region ICollection
   readonly bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

   bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item) {
      return Contains(item.Key, item.Value);
   }

   void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item) {
      _ = TryAdd(item.Key, item.Value);
   }

   bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item) {
      return Remove(item.Key, item.Value);
   }
   #endregion
}

public struct ValueMap<TKey, TValue>: IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>
where TKey: notnull {
   internal ValueMap<TKey, TValue, DefaultEqualer<TKey>> Impl = new(keyEqualer: new());

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueMap () { }

   public readonly int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.Count; }

   public TValue this [TKey key] {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl[key];
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Impl[key] = value;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly MapIterator<TKey, TValue> Iterate () {
      return Impl.Iterate();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly CollectionEnumerator<KeyValuePair<TKey, TValue>, MapIterator<TKey, TValue>> GetEnumerator () {
      return Impl.GetEnumerator();
   }

   readonly IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator () {
      return GetEnumerator().Box();
   }

   readonly IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator().Box();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool ContainsKey (TKey key) {
      return Impl.ContainsKey(key);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TValueEqualer> (TKey key, TValue value, TValueEqualer valueEqualer) where TValueEqualer: IEqualityComparer<TValue> {
      return Impl.Contains(key, value, valueEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TValueEqualer> (TKey key, TValue value, ProvideEqualer<TValue, TValueEqualer> provideValueEqualer)
   where TValueEqualer: IEqualityComparer<TValue> {
      return Impl.Contains(key, value, provideValueEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (TKey key, TValue value) {
      return Impl.Contains(key, value);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetValue (TKey key, [MaybeNullWhen(false)] out TValue value) {
      return Impl.TryGetValue(key, out value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public readonly void CopyTo (Span<KeyValuePair<TKey, TValue>> destination) {
      Impl.CopyTo(destination);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      Impl.CopyTo(array, arrayIndex);
   }

   readonly IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.CopyKeys(); }
   readonly ICollection<TKey> IDictionary<TKey, TValue>.Keys { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.CopyKeys(); }

   readonly IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.CopyValues();
   }

   readonly ICollection<TValue> IDictionary<TKey, TValue>.Values { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.CopyValues(); }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (TKey key, TValue value) {
      Impl.Add(key, value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (TKey key, TValue value) {
      return Impl.TryAdd(key, value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (TKey key) {
      return Impl.Remove(key);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TValueEqualer> (TKey key, TValue value, TValueEqualer valueEqualer) where TValueEqualer: IEqualityComparer<TValue> {
      return Impl.Remove(key, value, valueEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TValueEqualer> (TKey key, TValue value, ProvideEqualer<TValue, TValueEqualer> provideValueEqualer)
   where TValueEqualer: IEqualityComparer<TValue> {
      return Impl.Remove(key, value, provideValueEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (TKey key, TValue value) {
      return Impl.Remove(key, value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear () {
      Impl.Clear();
   }

   #region ICollection
   readonly bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

   bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> item) {
      return Contains(item.Key, item.Value);
   }

   void ICollection<KeyValuePair<TKey, TValue>>.Add (KeyValuePair<TKey, TValue> item) {
      _ = TryAdd(item.Key, item.Value);
   }

   bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> item) {
      return Remove(item.Key, item.Value);
   }
   #endregion
}
