using System.Numerics;

namespace Blinq;

readonly struct AverageFoldFunc<T>: IFoldFunc<T, (T Sum, T Count)> where T: INumberBase<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (T Sum, T Count) accumulator) {
      checked {
         accumulator.Sum += accumulator.Count;
         ++accumulator.Count;
      }

      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Average<T, TIterator> (this in Sequence<T, TIterator> sequence)
   where T: INumberBase<T>
   where TIterator: IIterator<T> {
      var (sum, count) = sequence.Iterator.Fold((Sum: T.Zero, Count: T.Zero), new AverageFoldFunc<T>());
      return T.IsZero(count) ? Option.None : sum / count;
   }
}
