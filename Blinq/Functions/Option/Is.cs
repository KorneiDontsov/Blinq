namespace Blinq;

public static partial class Option {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Is<T> (this in Option<T> option, [MaybeNullWhen(false)] out T value) {
      value = option.ValueOrDefault!;
      return option.HasValue;
   }
}
