using System.Diagnostics.CodeAnalysis;

namespace Blinq;

/// <summary>Supports a simple iteration over a sequence.</summary>
/// <typeparam name="T">The type of elements of a sequence.</typeparam>
[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IIterator<T> {
   TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated>;
}
