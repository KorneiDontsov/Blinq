namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
[ReadOnly(true)]
public interface IPredicate<T> {
   bool Invoke (T item);
}
