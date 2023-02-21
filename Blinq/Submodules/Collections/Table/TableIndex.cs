namespace Blinq.Collections;

readonly struct TableIndex {
   const int NoneValue = -1;
   const int UndefinedValue = -2;

   readonly int Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   TableIndex (int value) {
      Value = value;
   }

   public static TableIndex None { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(NoneValue); }
   public static TableIndex Undefined { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => new(UndefinedValue); }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TableIndex (int index) {
      return new TableIndex(index);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator int (TableIndex index) {
      return index.Value;
   }

   public bool HasValue { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Value > NoneValue; }
   public bool IsDefined { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Value >= NoneValue; }
}
