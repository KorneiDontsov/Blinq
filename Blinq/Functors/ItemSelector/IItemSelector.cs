using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IItemSelector<TIn, TOut> {
   TOut Invoke (TIn item);
}

public readonly struct FuncItemSelector<TIn, TOut>: IItemSelector<TIn, TOut> {
   readonly Func<TIn, TOut> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FuncItemSelector (Func<TIn, TOut> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TOut Invoke (TIn item) {
      return Func(item);
   }
}
