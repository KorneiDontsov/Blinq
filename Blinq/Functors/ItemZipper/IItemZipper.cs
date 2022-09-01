using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IItemZipper<TIn1, TIn2, TOut> {
   TOut Invoke (TIn1 item1, TIn2 item2);
}