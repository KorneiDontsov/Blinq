using System.Collections.Generic;

namespace Blinq;

public readonly struct NotEqualItemPredicate<T, TEqualer>: IItemPredicate<T> where TEqualer: IEqualityComparer<T> {
   readonly T Value;
   readonly TEqualer Equaler;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NotEqualItemPredicate (T value, TEqualer equaler) {
      Value = value;
      Equaler = equaler;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return !Equaler.Equals(Value, item);
   }
}
