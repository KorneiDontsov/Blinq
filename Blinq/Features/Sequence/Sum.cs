using System.Numerics;

namespace Blinq;

readonly struct SumFoldFunc<T>: IFoldFunc<T, T> where T: INumberBase<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulator) {
      checked {
         accumulator += item;
      }

      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Sum<T, TIterator> (this in Sequence<T, TIterator> sequence)
   where T: INumberBase<T>
   where TIterator: IIterator<T> {
      return sequence.Iterator.Fold(T.Zero, new SumFoldFunc<T>());
   }
}
