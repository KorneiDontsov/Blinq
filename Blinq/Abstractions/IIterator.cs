namespace Blinq;

/// <seealso cref="IIterator{T}.Fold{TAccumulator, TFoldFunc}" />
[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
[ReadOnly(true)]
public interface IFoldFunc<T, TAccumulator> {
   bool Invoke (T item, ref TAccumulator accumulator);
}

/// <summary>Supports a iteration over a sequence.</summary>
/// <typeparam name="T">The type of elements of a sequence.</typeparam>
public interface IIterator<T> {
   /// <summary>
   ///    Applies a function as long as it returns <see langword="false" />, producing a single, final value.
   /// </summary>
   /// <param name="seed">Initial value of accumulator.</param>
   /// <param name="func">A function to invoke on every iteration as long as it returns <see langword="false" />.</param>
   /// <typeparam name="TAccumulator">The type of the accumulator value.</typeparam>
   /// <typeparam name="TFoldFunc">The type of the accumulator function.</typeparam>
   /// <returns>The final accumulator value.</returns>
   TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator>;
}
