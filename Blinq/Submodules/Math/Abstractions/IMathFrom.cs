using System.Diagnostics.CodeAnalysis;

namespace Blinq.Math;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IMathFrom<T, TFrom>: IMathContract<T> {
   T From (TFrom n);
}
