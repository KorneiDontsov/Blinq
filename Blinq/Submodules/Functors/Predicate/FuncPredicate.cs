namespace Blinq.Functors;

public readonly struct FuncPredicate<T>: IPredicate<T> {
   readonly Func<T, bool> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FuncPredicate (Func<T, bool> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return Func(item);
   }
}
