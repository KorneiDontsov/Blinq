namespace Blinq;

public readonly record struct NumeratedItem<T>(T Value, int Position) {
   public readonly T Value = Value;
   public readonly int Position = Position;
}
