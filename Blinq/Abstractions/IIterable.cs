using System.Diagnostics.CodeAnalysis;

namespace Blinq;

/// <summary>Exposes the iterator, which supports a simple iteration over a sequence.</summary>
/// <typeparam name="T">The type of elements of a sequence.</typeparam>
/// <typeparam name="TIterator">The type of the iterator to expose.</typeparam>
[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IIterable<T, TIterator> where TIterator: IIterator<T> {
   TIterator CreateIterator ();
}
