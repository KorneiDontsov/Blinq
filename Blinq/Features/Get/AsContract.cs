namespace Blinq;

public static partial class Get<T> {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<T, TValue> AsContract<TValue> (TValue value) where TValue: T {
      return value;
   }
}
