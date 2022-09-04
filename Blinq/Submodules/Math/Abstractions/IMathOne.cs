using System.Diagnostics.CodeAnalysis;

namespace Blinq.Math;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IMathOne<T>: IMath<T> {
   T One ();
}
