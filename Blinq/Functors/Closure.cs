namespace Blinq;

public readonly struct Closure<TResult, TState, TImpl>
   : IFunctor<TResult>
where TImpl: IFunctor<TState, TResult> {
   public required TImpl impl { get; init; }

   readonly TState _state;
   public required TState state { get => this._state; init => this._state = value; }

   public TResult Invoke () {
      return this.impl.Invoke(in this._state);
   }
}

public readonly struct Closure<TState, TArg, TResult, TImpl>
   : IFunctor<TArg, TResult>
where TImpl: IFunctor<TState, TArg, TResult> {
   readonly TState _state;
   public required TState state { get => this._state; init => this._state = value; }

   public required TImpl impl { get; init; }

   public TResult Invoke (in TArg arg) {
      return this.impl.Invoke(in this._state, in arg);
   }
}

public readonly struct Closure<TState, TArg1, TArg2, TResult, TImpl>
   : IFunctor<TArg1, TArg2, TResult>
where TImpl: IFunctor<TState, TArg1, TArg2, TResult> {
   readonly TState _state;
   public required TState state { get => this._state; init => this._state = value; }

   public required TImpl impl { get; init; }

   public TResult Invoke (in TArg1 arg1, in TArg2 arg2) {
      return this.impl.Invoke(in this._state, in arg1, in arg2);
   }
}
