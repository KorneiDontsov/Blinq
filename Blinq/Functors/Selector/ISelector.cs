using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface ISelector<TIn, TOut> {
   TOut Invoke (TIn item);
}
