namespace Blinq.Collections;

struct TableIteratorImpl<TEntry, TOut, TSelectOutput>: IIterator<TOut> where TSelectOutput: ISelectOutputOfTableIterator<TEntry, TOut> {
   readonly TableCell<TEntry>[] Cells;
   readonly int Size;
   int FreeCount;
   int Index;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal TableIteratorImpl (TableCell<TEntry>[] cells, int size, int freeCount) {
      Cells = cells;
      Size = size;
      FreeCount = freeCount;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      foreach (ref var cell in Cells.AsSpan(Index, Size)) {
         ++Index;
         if (!cell.Previous.IsDefined) {
            --FreeCount;
         } else {
            item = TSelectOutput.Invoke(cell.Entry);
            return true;
         }
      }

      item = default;
      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TOut, TAccumulator> {
      foreach (ref var cell in Cells.AsSpan(Index, Size)) {
         ++Index;
         if (!cell.Previous.IsDefined) {
            --FreeCount;
         } else if (fold.Invoke(TSelectOutput.Invoke(cell.Entry), ref accumulator)) {
            break;
         }
      }

      return accumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Size - FreeCount - Index;
      return true;
   }
}
