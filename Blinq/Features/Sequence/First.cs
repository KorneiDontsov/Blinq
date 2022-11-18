namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> First<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      var iterator = sequence.Iterator;
      return Sequence<T>.Pop(ref iterator);
   }
}
