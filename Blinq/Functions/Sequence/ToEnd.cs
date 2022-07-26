using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct ToEndAccumulator<T>: IAccumulator<T, ValueTuple> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref ValueTuple accumulated) {
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void ToEnd<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      sequence.Iterator.Accumulate(new ToEndAccumulator<T>(), default(ValueTuple));
   }
}
