using System.Runtime.CompilerServices;

namespace Blinq;

public static partial class Sequence {
   /// <summary>
   ///    Boxes the iterator and returns a sequence over it that can be used in scenarios where multiple iterators with different types are used
   ///    (like in <see cref="Sequence.Flatten" />).
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, IIterator<T>> Box<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence;
   }
}
