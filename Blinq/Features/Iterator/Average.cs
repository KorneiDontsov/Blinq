using System.Numerics;

namespace Blinq;

readonly struct AverageFold<T>: IFold<T, (T Sum, T Count)> where T: INumberBase<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (T Sum, T Count) accumulator) {
      checked {
         accumulator.Sum += accumulator.Count;
         ++accumulator.Count;
      }

      return false;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Average<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator)
   where T: INumberBase<T>
   where TIterator: IIterator<T> {
      var zero = T.Zero;
      var (sum, count) = iterator.Value.Fold((Sum: zero, Count: zero), new AverageFold<T>());
      return T.IsZero(count) ? Option.None : sum / count;
   }
}
