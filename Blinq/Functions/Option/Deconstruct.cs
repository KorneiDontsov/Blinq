namespace Blinq;

public static partial class Option {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void Deconstruct<T> (this in Option<T> option, out bool hasValue, out T valueOrDefault) {
      hasValue = option.HasValue;
      valueOrDefault = option.ValueOrDefault!;
   }
}
