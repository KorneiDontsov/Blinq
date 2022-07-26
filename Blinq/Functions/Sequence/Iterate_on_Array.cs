using System.Runtime.CompilerServices;

namespace Blinq;

/// <inheritdoc />
/// <summary>An array iterator.</summary>
public struct ArrayIterator<T>: IIterator<T> {
   readonly T[] Array;
   int Index;

   public ArrayIterator (T[] array) {
      Array = array;
      Index = 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated> {
      foreach (var item in Array.AsSpan(Index)) {
         ++Index;
         if (accumulator.Invoke(item, ref seed)) break;
      }

      return seed;
   }
}

public static partial class Sequence {
   /// <summary>Creates a sequence over <paramref name="array" />.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, ArrayIterator<T>> Iterate<T> (this T[] array) {
      return new Sequence<T, ArrayIterator<T>>(new ArrayIterator<T>(array), array.Length);
   }
}
