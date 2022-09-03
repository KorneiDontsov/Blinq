using System.Diagnostics.CodeAnalysis;

namespace Blinq.Math;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IMathZero<T>: IMathContract<T> {
   T Zero ();
}
