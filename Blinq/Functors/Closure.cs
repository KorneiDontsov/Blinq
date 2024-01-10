namespace Blinq;

public readonly struct Closure<TResult, TState, TImpl>
   : IFunctor<TResult>
where TImpl: IFunctor<TState, TResult> {
   public required TImpl impl { get; init; }

   readonly TState _state;
   public required TState state { get => _state; init => _state = value; }

   public TResult Invoke () {
      return impl.Invoke(in _state);
   }
}

public readonly struct Closure<TArg, TResult, TState, TImpl>
   : IFunctor<TArg, TResult>
where TImpl: IFunctor<TArg, TState, TResult> {
   readonly TState _state;
   public required TState state { get => _state; init => _state = value; }

   public required TImpl impl { get; init; }

   public TResult Invoke (in TArg arg) {
      return impl.Invoke(in arg, in _state);
   }
}

public readonly struct Closure<TArg1, TArg2, TResult, TState, TImpl>
   : IFunctor<TArg1, TArg2, TResult>
where TImpl: IFunctor<TArg1, TArg2, TState, TResult> {
   readonly TState _state;
   public required TState state { get => _state; init => _state = value; }

   public required TImpl impl { get; init; }

   public TResult Invoke (in TArg1 arg1, in TArg2 arg2) {
      return impl.Invoke(in arg1, in arg2, in _state);
   }
}
