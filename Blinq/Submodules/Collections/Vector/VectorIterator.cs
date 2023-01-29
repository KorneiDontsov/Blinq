namespace Blinq.Collections;

public struct VectorIterator<T>: IIterator<T> {
   readonly T[] Items;
   readonly int Size;
   int Index;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public VectorIterator (T[] items, int size) {
      Items = items;
      Size = size;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      foreach (var item in Items.AsSpan(Index, Size)) {
         ++Index;
         if (func.Invoke(item, ref seed)) break;
      }

      return seed;
   }
}
