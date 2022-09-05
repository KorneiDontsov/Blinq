namespace Blinq;

/// <inheritdoc />
/// <summary>An array iterator.</summary>
public struct ArrayIterator<T>: IIterator<T> {
   readonly T[] Array;
   int Index;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ArrayIterator (T[] array) {
      Array = array;
      Index = 0;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      foreach (var item in Array.AsSpan(Index)) {
         ++Index;
         if (func.Invoke(item, ref seed)) break;
      }

      return seed;
   }
}

public static partial class Sequence {
   /// <summary>Creates a sequence over <paramref name="array" />.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, ArrayIterator<T>> Seq<T> (this T[] array) {
      return Sequence<T>.Create(new ArrayIterator<T>(array), array.Length);
   }
}
