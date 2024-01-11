using System;
using System.Runtime.CompilerServices;

namespace Blinq;

public readonly struct Functor<TResult>: IFunctor<TResult> {
   public required Func<TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke () {
      return this.@delegate();
   }
}

public readonly struct Functor<TArg, TResult>
   : IFunctor<TArg, TResult> {
   public required Func<TArg, TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke (in TArg arg) {
      return this.@delegate(arg);
   }
}

public readonly struct Functor<TArg1, TArg2, TResult>
   : IFunctor<TArg1, TArg2, TResult> {
   public required Func<TArg1, TArg2, TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke (in TArg1 arg1, in TArg2 arg2) {
      return this.@delegate(arg1, arg2);
   }
}

public readonly struct Functor<TArg1, TArg2, TArg3, TResult>
   : IFunctor<TArg1, TArg2, TArg3, TResult> {
   public required Func<TArg1, TArg2, TArg3, TResult> @delegate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TResult Invoke (in TArg1 arg1, in TArg2 arg2, in TArg3 arg3) {
      return this.@delegate(arg1, arg2, arg3);
   }
}

public static class Functor {
   public static Pin<IFunctor<TResult>, Functor<TResult>> New<TResult> (Func<TResult> @delegate) {
      return new() { value = new() { @delegate = @delegate } };
   }

   public static Pin<IFunctor<TArg, TResult>, Functor<TArg, TResult>> New<TArg, TResult> (
      Func<TArg, TResult> @delegate
   ) {
      return new() { value = new() { @delegate = @delegate } };
   }

   public static Pin<
      IFunctor<TArg1, TArg2, TResult>,
      Functor<TArg1, TArg2, TResult>
   > New<TArg1, TArg2, TResult> (Func<TArg1, TArg2, TResult> @delegate) {
      return new() { value = new() { @delegate = @delegate } };
   }

   public static Pin<
      IFunctor<TArg1, TArg2, TArg3, TResult>,
      Functor<TArg1, TArg2, TArg3, TResult>
   > New<TArg1, TArg2, TArg3, TResult> (Func<TArg1, TArg2, TArg3, TResult> @delegate) {
      return new() { value = new() { @delegate = @delegate } };
   }

   public static Pin<IFunctor<TArg, TResult>, ByRefFunctor<TArg, TResult>> New<TArg, TResult> (
      ByRefFunc<TArg, TResult> @delegate
   ) {
      return new() { value = new() { @delegate = @delegate } };
   }

   public static Pin<
      IFunctor<TArg1, TArg2, TResult>,
      ByRefFunctor<TArg1, TArg2, TResult>
   > New<TArg1, TArg2, TResult> (ByRefFunc<TArg1, TArg2, TResult> @delegate) {
      return new() { value = new() { @delegate = @delegate } };
   }

   public static Pin<
      IFunctor<TArg1, TArg2, TArg3, TResult>,
      ByRefFunctor<TArg1, TArg2, TArg3, TResult>
   > New<TArg1, TArg2, TArg3, TResult> (ByRefFunc<TArg1, TArg2, TArg3, TResult> @delegate) {
      return new() { value = new() { @delegate = @delegate } };
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Invoke<TResult, TFunctor> (this Pin<IFunctor<TResult>, TFunctor> functor)
   where TFunctor: IFunctor<TResult> {
      return functor.value.Invoke();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Invoke<TArg, TResult, TFunctor> (
      this Pin<IFunctor<TArg, TResult>, TFunctor> functor,
      in TArg arg
   ) where TFunctor: IFunctor<TArg, TResult> {
      return functor.value.Invoke(in arg);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Invoke<TArg1, TArg2, TResult, TFunctor> (
      this Pin<IFunctor<TArg1, TArg2, TResult>, TFunctor> functor,
      in TArg1 arg1,
      in TArg2 arg2
   ) where TFunctor: IFunctor<TArg1, TArg2, TResult> {
      return functor.value.Invoke(in arg1, in arg2);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Invoke<TArg1, TArg2, TArg3, TResult, TFunctor> (
      this Pin<IFunctor<TArg1, TArg2, TArg3, TResult>, TFunctor> functor,
      in TArg1 arg1,
      in TArg2 arg2,
      in TArg3 arg3
   ) where TFunctor: IFunctor<TArg1, TArg2, TArg3, TResult> {
      return functor.value.Invoke(in arg1, in arg2, in arg3);
   }

   public static Pin<
      IFunctor<TResult>,
      Closure<TResult, TState, TImpl>
   > Bind<TResult, TState, TImpl> (this Pin<IFunctor<TState, TResult>, TImpl> impl, TState state)
   where TImpl: IFunctor<TState, TResult> {
      return new() { value = new() { impl = impl, state = state } };
   }

   public static Pin<
      IFunctor<TArg, TResult>,
      Closure<TArg, TResult, TState, TImpl>
   > Bind<TArg, TResult, TState, TImpl> (
      this Pin<IFunctor<TArg, TState, TResult>, TImpl> impl,
      TState state
   ) where TImpl: IFunctor<TArg, TState, TResult> {
      return new() { value = new() { impl = impl, state = state } };
   }

   public static Pin<
      IFunctor<TArg1, TArg2, TResult>,
      Closure<TArg1, TArg2, TResult, TState, TImpl>
   > Bind<TArg1, TArg2, TResult, TState, TImpl> (
      this Pin<IFunctor<TArg1, TArg2, TState, TResult>, TImpl> impl,
      TState state
   ) where TImpl: IFunctor<TArg1, TArg2, TState, TResult> {
      return new() { value = new() { impl = impl, state = state } };
   }
}
