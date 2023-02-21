namespace Blinq.Collections;

public struct TableEnumerator<TEntry>: ILightEnumerator<TEntry> where TEntry: notnull {
   readonly TableCell<TEntry>[] Cells;
   readonly int Size;
   int Index;
   public TEntry Current { get; private set; } = default!;

   internal TableEnumerator (TableCell<TEntry>[] cells, int size) {
      Cells = cells;
      Size = size;
   }

   public bool MoveNext () {
      foreach (ref var cell in Cells.AsSpan(Index, Size)) {
         ++Index;
         if (cell.Previous.IsDefined) {
            Current = cell.Entry;
            return true;
         }
      }

      Current = default!;
      return false;
   }
}
