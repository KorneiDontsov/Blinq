using System.Diagnostics.CodeAnalysis;

namespace Blinq.Math;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IMathOne<T>: IMathContract<T> {
   T One ();
}
