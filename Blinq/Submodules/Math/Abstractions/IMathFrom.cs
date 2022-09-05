namespace Blinq.Math;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IMathFrom<T, TFrom>: IMath<T> {
   T From (TFrom n);
}
