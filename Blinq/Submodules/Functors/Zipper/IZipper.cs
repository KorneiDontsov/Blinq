namespace Blinq.Functors;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IZipper<TIn1, TIn2, TOut> {
   TOut Invoke (TIn1 item1, TIn2 item2);
}
