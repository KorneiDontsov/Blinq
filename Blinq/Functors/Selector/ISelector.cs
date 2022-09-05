namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
[ReadOnly(true)]
public interface ISelector<TIn, TOut> {
   TOut Invoke (TIn item);
}
