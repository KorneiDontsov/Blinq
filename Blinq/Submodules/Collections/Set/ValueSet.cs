namespace Blinq.Collections;

public struct ValueSet<T, TEqualer>
where T: notnull
where TEqualer: IEqualityComparer<T> {
   internal ValueTable<T, T, TEqualer, SetKeySelector<T>> Table;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueSet (TEqualer equaler) {
      Table = new(equaler);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T item) {
      return this.Match(item).TryAdd();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      var match = this.Match(item);
      if (match.ItemIsPresent) Get.Throw<ArgumentException>();
      match.Add();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T item) {
      return this.Match(item).TryRemove();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Table.EnsureCapacity(minCapacity);
   }
}

public struct ValueSet<T> where T: notnull {
   internal ValueSet<T, DefaultEqualer<T>> Impl = new(equaler: new());

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueSet () { }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd (T item) {
      return Impl.TryAdd(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      Impl.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T item) {
      return Impl.Remove(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void EnsureCapacity (int minCapacity) {
      Impl.EnsureCapacity(minCapacity);
   }
}
