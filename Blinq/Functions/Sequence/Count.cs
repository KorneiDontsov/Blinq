using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct CountAccumulator<T>: IAccumulator<T, int> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref int accumulated) {
      ++accumulated;
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Count<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Count switch {
         (true, var count) => count,
         _ => sequence.Iterator.Accumulate(new CountAccumulator<T>(), 0),
      };
   }
}
