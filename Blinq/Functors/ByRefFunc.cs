namespace Blinq;

public delegate TResult ByRefFunc<TArg1, out TResult> (in TArg1 arg);

public delegate TResult ByRefFunc<TArg1, TArg2, out TResult> (in TArg1 arg1, in TArg2 arg2);

public delegate TResult ByRefFunc<TArg1, TArg2, TArg3, out TResult> (
   in TArg1 arg1,
   in TArg2 arg2,
   in TArg3 arg3
);
