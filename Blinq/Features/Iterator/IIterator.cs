namespace Blinq;

/// <summary>Supports a iteration over a sequence.</summary>
/// <typeparam name="T">The type of elements of a sequence.</typeparam>
public interface IIterator<T> {
   bool TryPop ([MaybeNullWhen(false)] out T item);

   /// <summary>
   ///    Applies a function as long as it returns <see langword="false" />, producing a single, final value.
   /// </summary>
   /// <param name="accumulator">Initial value of accumulator.</param>
   /// <param name="fold">A function to invoke on every iteration as long as it returns <see langword="false" />.</param>
   /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
   /// <typeparam name="TFold">The type of the accumulator function.</typeparam>
   /// <returns>The final accumulator value.</returns>
   TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator>;

   [Pure] bool TryGetCount (out int count);
}
