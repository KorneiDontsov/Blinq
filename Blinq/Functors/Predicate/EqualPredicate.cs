namespace Blinq;

public readonly struct EqualPredicate<T, TEqualer>: IPredicate<T> where TEqualer: IEqualityComparer<T> {
   readonly T Value;
   readonly TEqualer Equaler;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public EqualPredicate (T value, TEqualer equaler) {
      Value = value;
      Equaler = equaler;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return Equaler.Equals(Value, item);
   }
}
