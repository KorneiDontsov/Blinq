namespace Blinq.Collections;

public ref struct SetItemMatch<T> {
   TableEntryMatchImpl<T> Impl;
   public T ExpectedItem { get; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal SetItemMatch (TableEntryMatchImpl<T> impl, T expectedItem) {
      Impl = impl;
      ExpectedItem = expectedItem;
   }

   public readonly bool ItemIsPresent { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.EntryIsPresent; }
   public readonly T ActualItem { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Impl.Entry; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal void DoAdd () {
      Impl.Add(ExpectedItem);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add () {
      Impl.CheckNotExists();
      DoAdd();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryAdd () {
      if (Impl.EntryIsPresent) {
         return false;
      } else {
         DoAdd();
         return true;
      }
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddOrReplace () {
      if (Impl.EntryIsPresent) {
         Impl.EntryRef = ExpectedItem;
      } else {
         DoAdd();
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
