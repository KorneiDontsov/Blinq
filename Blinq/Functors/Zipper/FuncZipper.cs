namespace Blinq;

public readonly struct FuncZipper<TIn1, TIn2, TOut>: IZipper<TIn1, TIn2, TOut> {
   readonly Func<TIn1, TIn2, TOut> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FuncZipper (Func<TIn1, TIn2, TOut> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TOut Invoke (TIn1 item1, TIn2 item2) {
      return Func(item1, item2);
   }
}
