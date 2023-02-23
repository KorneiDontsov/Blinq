namespace Blinq.Functors;

public readonly struct FuncSelector<TIn, TOut>: ISelector<TIn, TOut> {
   readonly Func<TIn, TOut> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FuncSelector (Func<TIn, TOut> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TOut Invoke (TIn input) {
      return Func(input);
   }
}
