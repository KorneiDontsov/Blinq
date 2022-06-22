namespace Blinq;

public static partial class Option {
   public static void Deconstruct<T> (this in Option<T> option, out bool hasValue, out T valueOrDefault) {
      hasValue = option.HasValue;
      valueOrDefault = option.ValueOrDefault!;
   }
}
