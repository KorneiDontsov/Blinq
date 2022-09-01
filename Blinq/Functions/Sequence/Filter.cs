namespace Blinq;

public readonly partial struct FilterContinuation<T, TIterator> where TIterator: IIterator<T> {
   readonly Sequence<T, TIterator> Sequence;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterContinuation (Sequence<T, TIterator> sequence) {
      Sequence = sequence;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static FilterContinuation<T, TIterator> Filter<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return new FilterContinuation<T, TIterator>(sequence);
   }
}
