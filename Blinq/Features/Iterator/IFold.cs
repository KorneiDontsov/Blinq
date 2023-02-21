namespace Blinq;

/// <seealso cref="IIterator{T}.Fold{TAccumulator, TFold}" />
[ReadOnly(true)]
public interface IFold<T, TAccumulator> {
   [Pure] bool Invoke (T item, ref TAccumulator accumulator);
}
