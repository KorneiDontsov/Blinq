using System.Collections;
using Blinq.Functors;

namespace Blinq.Collections;

public abstract class Vector<T>: IReadOnlyCollection<T>, ICollection<T> {
   private protected ValueVector<T> Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   private protected Vector (ValueVector<T> value) {
      Value = value;
   }

   [Pure] public int Count { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Value.Count; }

   [Pure] public T this [int index] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Value.At(index); }

   [Pure] public T this [Index index] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Value.At(index); }


   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T At (int index) {
      return Value.At(index);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T At (Index index) {
      return Value.At(index);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ReadOnlySpan<T> AsReadOnlySpan () {
      return Value.AsReadOnlySpan();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public VectorIterator<T> Iter () {
      return Value.Iter();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public VectorEnumerator<T> GetEnumerator () {
      return Value.GetEnumerator();
   }

   IEnumerator<T> IEnumerable<T>.GetEnumerator () {
      return LightEnumeratorWrap<T>.Create(GetEnumerator());
   }

   IEnumerator IEnumerable.GetEnumerator () {
      return LightEnumeratorWrap<T>.Create(GetEnumerator());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void CopyTo (T[] array, int arrayIndex) {
      Value.CopyTo(array, arrayIndex);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T[] ToArray () {
      return Value.ToArray();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public MutVector<T> Copy () {
      return Value.ToMutable();
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueVector<T> ToValue () {
      return Value.Copy();
   }

   [Pure] public abstract ImmVector<T> ToImmutable ();

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int IndexOf<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      return Value.IndexOf(predicate);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int IndexOf (Func<T, bool> predicate) {
      return Value.IndexOf(predicate);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int IndexOf<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return Value.IndexOf(item, equaler);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int IndexOf<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Value.IndexOf(item, provideEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int IndexOf (T item) {
      return Value.IndexOf(item);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int LastIndexOf<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      return Value.IndexOf(predicate);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int LastIndexOf (Func<T, bool> predicate) {
      return Value.IndexOf(predicate);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int LastIndexOf<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return Value.IndexOf(item, equaler);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int LastIndexOf<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Value.IndexOf(item, provideEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int LastIndexOf (T item) {
      return Value.IndexOf(item);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      return Value.Contains(predicate);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (Func<T, bool> predicate) {
      return Value.Contains(predicate);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return Value.Contains(item, equaler);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Value.Contains(item, provideEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Contains (T item) {
      return Value.Contains(item);
   }

   #region ICollection
   bool ICollection<T>.IsReadOnly => true;

   void ICollection<T>.Add (T item) {
      throw new NotSupportedException();
   }

   bool ICollection<T>.Remove (T item) {
      throw new NotSupportedException();
   }

   void ICollection<T>.Clear () {
      throw new NotSupportedException();
   }
   #endregion
}
