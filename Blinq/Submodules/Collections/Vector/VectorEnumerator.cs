namespace Blinq.Collections;

public struct VectorEnumerator<T>: ICollectionEnumerator<T> {
   readonly T[] Items;
   readonly int Size;
   int Index;

   public T Current { get; private set; } = default!;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal VectorEnumerator (T[] items, int size) {
      Items = items;
      Size = size;
   }

   bool MoveNextRare () {
      Index = Size + 1;
      Current = default!;
      return false;
   }

   public bool MoveNext () {
      if (Index < Size) {
         Current = Items[Index];
         Index++;
         return true;
      }

      return MoveNextRare();
   }
}
