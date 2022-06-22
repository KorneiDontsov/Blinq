namespace Blinq;

/// <summary>
///    Represents a sequence and provides its iterator.
/// </summary>
/// <typeparam name="T">The type of elements of a sequence.</typeparam>
/// <typeparam name="TIterator">The type of the iterator.</typeparam>
public readonly struct Sequence<T, TIterator> where TIterator: IIterator<T> {
   /// <summary>The iterator of the sequence.</summary>
   public readonly TIterator Iterator;

   /// <summary>
   ///    The estimated count of elements of the sequence.
   ///    Zero, if the sequence is empty or the count of elements is not known.
   /// </summary>
   public readonly int Count;

   public Sequence (TIterator iterator, int count = 0) {
      Iterator = iterator;
      Count = count;
   }

   public static implicit operator Sequence<T, TIterator> (TIterator iterator) {
      return new Sequence<T, TIterator>(iterator);
   }

   public static implicit operator TIterator (Sequence<T, TIterator> sequence) {
      return sequence.Iterator;
   }

   /// <summary>
   ///    Boxes the iterator and returns a sequence over it that can be used in scenarios where multiple iterators with different types are used
   ///    (like in <see cref="Sequence.Flatten" />).
   /// </summary>
   public static implicit operator Sequence<T, IIterator<T>> (Sequence<T, TIterator> sequence) {
      return new Sequence<T, IIterator<T>>(sequence.Iterator, sequence.Count);
   }
}

/// <summary>
///    Provides high-performance allocation-free alternatives of "LINQ to objects" methods.
/// </summary>
public static partial class Sequence { }
