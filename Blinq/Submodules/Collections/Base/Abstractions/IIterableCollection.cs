using System.Collections;
using System.Collections.Generic;

namespace Blinq.Collections;

/// <summary>
///    Represents a collection that additionally implements <see cref="IIterable{T,TIterator}" /> and can be iterated.
/// </summary>
/// <typeparam name="T">The type of elements of the collection.</typeparam>
/// <typeparam name="TIterator">The type of the iterator to expose.</typeparam>
public interface IIterableCollection<T, TIterator>: IIterable<T, TIterator>, IReadOnlyCollection<T>, ICollection<T> where TIterator: IIterator<T> {
   /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
   public new int Count { get; }

   int IReadOnlyCollection<T>.Count => Count;
   int ICollection<T>.Count => Count;

   bool ICollection<T>.IsReadOnly => true;

   IEnumerator IEnumerable.GetEnumerator () {
      return GetEnumerator();
   }

   bool Contains<TEqualer> (T item, TEqualer equaler) where TEqualer: IEqualityComparer<T>;

   bool ICollection<T>.Contains (T item) {
      return Contains(item, Equaler.Default<T>());
   }

   void ICollection<T>.Add (T item) {
      throw new NotSupportedException();
   }

   bool ICollection<T>.Remove (T item) {
      throw new NotSupportedException();
   }

   void ICollection<T>.Clear () {
      throw new NotSupportedException();
   }
}
