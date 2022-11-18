namespace Blinq.Functors;

[ReadOnly(true)]
public interface IPredicate<T> {
   [Pure] bool Invoke (T item);
}
