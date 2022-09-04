using System.Diagnostics.CodeAnalysis;

namespace Blinq.Math;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IMathZero<T>: IMath<T> {
   T Zero ();
}
