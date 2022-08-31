namespace Blinq;

public readonly struct FuncItemPredicate<T>: IItemPredicate<T> {
   readonly Func<T, bool> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FuncItemPredicate (Func<T, bool> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return Func(item);
   }
}
