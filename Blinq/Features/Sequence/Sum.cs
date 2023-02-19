using System.Numerics;

namespace Blinq;

readonly struct SumFold<T>: IFold<T, T> where T: IAdditionOperators<T, T, T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulator) {
      checked {
         accumulator += item;
      }

      return false;
   }
}

public static partial class Iterator {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Sum<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator)
   where T: INumberBase<T>
   where TIterator: IIterator<T> {
      return iterator.Value.Fold(T.Zero, new SumFold<T>());
   }
}
