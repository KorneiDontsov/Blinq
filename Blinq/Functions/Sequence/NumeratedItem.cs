namespace Blinq;

public readonly struct NumeratedItem<T> {
   public readonly T Value;
   public readonly int Position;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NumeratedItem (T value, int position) {
      Value = value;
      Position = position;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Deconstruct (out T value, out int position) {
      value = Value;
      position = Position;
   }
}
