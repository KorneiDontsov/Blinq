namespace Blinq;

public readonly partial struct Use<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<T, TValue> As<TValue> (TValue value) where TValue: T {
      return value;
   }
}
