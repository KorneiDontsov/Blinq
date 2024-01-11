using System.Runtime.CompilerServices;

namespace Blinq;

public readonly struct ByRefFunctor<TArg, TResult>
   : IFunctor<TArg, TResult> {
   public required ByRefFunc<TArg, TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke (in TArg arg) {
      return this.@delegate(in arg);
   }
}

public readonly struct ByRefFunctor<TArg1, TArg2, TResult>
   : IFunctor<TArg1, TArg2, TResult> {
   public required ByRefFunc<TArg1, TArg2, TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke (in TArg1 arg1, in TArg2 arg2) {
      return this.@delegate(in arg1, in arg2);
   }
}

public readonly struct ByRefFunctor<TArg1, TArg2, TArg3, TResult>
   : IFunctor<TArg1, TArg2, TArg3, TResult> {
   public required ByRefFunc<TArg1, TArg2, TArg3, TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke (in TArg1 arg1, in TArg2 arg2, in TArg3 arg3) {
      return this.@delegate(in arg1, in arg2, in arg3);
   }
}
