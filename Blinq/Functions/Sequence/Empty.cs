using System.Runtime.CompilerServices;

namespace Blinq;

public readonly struct EmptyIterator<T>: IIterator<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated> {
      return seed;
   }
}

public static partial class Sequence {
   /// <summary>Returns empty sequence of a specified type.</summary>
   /// <typeparam name="T">The type of elements of a sequence.</typeparam>
   /// <returns>Empty sequence.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, EmptyIterator<T>> Empty<T> () {
      return default;
   }
}
