namespace Blinq;

public static partial class Sequence {
   /// <summary>Returns the underlying iterator.</summary>
   /// <returns>This method exists only to make <see langword="foreach" /> work.</returns>
   public static TIterator GetEnumerator<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Iterator;
   }
}
