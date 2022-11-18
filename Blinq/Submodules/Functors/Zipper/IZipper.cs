namespace Blinq.Functors;

public interface IZipper<TIn1, TIn2, TOut> {
   [Pure] TOut Invoke (TIn1 item1, TIn2 item2);
}
