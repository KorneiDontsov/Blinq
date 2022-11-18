namespace Blinq;

public static partial class Get<T> {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Type<T, TImplementation> AsType<TImplementation> () where TImplementation: T {
      return new();
   }
}
