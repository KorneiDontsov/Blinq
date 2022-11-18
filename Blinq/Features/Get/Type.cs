namespace Blinq;

public static partial class Get {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Type<T> Type<T> () {
      return new();
   }
}
