using Blinq.Functors;

namespace Blinq.Collections;

public sealed class MutVector<T>: Vector<T>, ICollection<T>, IMutVectorInternal<T> {
   internal MutVector (ValueVector<T> value): base(value) { }
   public MutVector (): this(new ValueVector<T>()) { }
   public MutVector (int capacity): this(new ValueVector<T>(capacity)) { }

   bool ICollection<T>.IsReadOnly => false;

   public int Capacity {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Value.Capacity;
      [MethodImpl(MethodImplOptions.AggressiveInlining)] set => Value.Capacity = value;
   }

   public new ref T this [int index] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref Value[index]; }

   public new ref T this [Index index] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref Value[index]; }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Span<T> AsSpan () {
      return Value.AsSpan();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T[] MoveToArray () {
      return Value.MoveToArray();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public override ImmVector<T> ToImmutable () {
      return Value.ToImmutable();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ImmVector<T> Immute () {
      var result = ImmVector<T>.Create(Value);
      Value = new();
      return result;
   }

   bool IMutVectorInternal<T>.HasSame (T[] items) {
      return Value.HasSame(items);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      Value.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddRange (ICollection<T> collection) {
      Value.AddRange(collection);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddRange (IEnumerator<T> enumerator) {
      Value.AddRange(enumerator);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void AddRange (IEnumerable<T> enumerable) {
      Value.AddRange(enumerable);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Insert (int index, T item) {
      Value.Insert(index, item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Insert (Index index, T item) {
      Value.Insert(index, item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void InsertRange (int index, ICollection<T> collection) {
      Value.InsertRange(index, collection);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void InsertRange (Index index, ICollection<T> collection) {
      Value.InsertRange(index, collection);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void InsertRange (int index, IEnumerator<T> enumerator) {
      Value.InsertRange(index, enumerator);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void InsertRange (Index index, IEnumerator<T> enumerator) {
      Value.InsertRange(index, enumerator);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void InsertRange (int index, IEnumerable<T> enumerable) {
      Value.InsertRange(index, enumerable);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void InsertRange (Index index, IEnumerable<T> enumerable) {
      Value.InsertRange(index, enumerable);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void RemoveAt (int index) {
      Value.RemoveAt(index);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void RemoveAt (Index index) {
      Value.RemoveAt(index);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void RemoveRange (int index, int count) {
      Value.RemoveRange(index, count);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void RemoveRange (Range range) {
      Value.RemoveRange(range);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      return Value.Remove(predicate);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (Func<T, bool> predicate) {
      return Value.Remove(predicate);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T> {
      return Value.Remove(item, equaler);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove<TEqualer> (T item, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return Value.Remove(item, provideEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Remove (T item) {
      return Value.Remove(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int RemoveAll<TPredicate> (TPredicate predicate) where TPredicate: IPredicate<T> {
      return Value.RemoveAll(predicate);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Clear () {
      Value.Clear();
   }
}
