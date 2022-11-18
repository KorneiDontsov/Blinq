namespace Blinq.Functors;

[ReadOnly(true)]
public interface ISelector<TIn, TOut> {
   [Pure] TOut Invoke (TIn item);
}
