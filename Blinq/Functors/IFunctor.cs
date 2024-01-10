namespace Blinq;

public interface IFunctor<TResult> {
   TResult Invoke ();
}

public interface IFunctor<TArg, TResult> {
   TResult Invoke (in TArg arg);
}

public interface IFunctor<TArg1, TArg2, TResult> {
   TResult Invoke (in TArg1 arg1, in TArg2 arg2);
}

public interface IFunctor<TArg1, TArg2, TArg3, TResult> {
   TResult Invoke (in TArg1 arg1, in TArg2 arg2, in TArg3 arg3);
}
