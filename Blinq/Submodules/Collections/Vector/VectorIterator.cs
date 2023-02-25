namespace Blinq.Collections;

public struct VectorIterator<T>: IIterator<T> {
   readonly T[] Items;
   readonly int Size;
   int Index;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal VectorIterator (T[] items, int size) {
      Items = items;
      Size = size;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Index < Size) {
         item = Items[Index++];
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      foreach (var item in Items.AsSpan(Index, Size)) {
         ++Index;
         if (fold.Invoke(item, ref accumulator)) break;
      }

      return accumulator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Size - Index;
      return true;
   }
}
