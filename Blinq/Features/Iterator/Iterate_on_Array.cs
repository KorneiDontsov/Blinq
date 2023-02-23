namespace Blinq;

/// <inheritdoc />
/// <summary>An array iterator.</summary>
public struct ArrayIterator<T>: IIterator<T> {
   readonly T[] Array;
   int Index;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ArrayIterator (T[] array) {
      Array = array;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Index < Array.Length) {
         item = Array[Index++];
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      foreach (var item in Array.AsSpan(Index)) {
         ++Index;
         if (fold.Invoke(item, ref seed)) break;
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Array.Length - Index;
      return true;
   }
}

public static partial class Iterator {
   /// <summary>Creates a sequence over <paramref name="array" />.</summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, ArrayIterator<T>> Iterate<T> (this T[] array) {
      return new ArrayIterator<T>(array);
   }
}
