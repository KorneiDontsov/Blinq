using System.Diagnostics.CodeAnalysis;

namespace Blinq;

/// <remarks>
///    <b>Undefined behaviours</b> (the things you <b>must not</b> do with any iterator):
///    <list type="number">
///       <item>
///          Invoke <see cref="IIterator{T}.Current" /> before <see cref="IIterator{T}.MoveNext()" /> is invoked;
///       </item>
///       <item>
///          Invoke <see cref="IIterator{T}.Current" /> or <see cref="IIterator{T}.MoveNext()" /> after <see cref="IIterator{T}.MoveNext()" />
///          returned <see langword="false" />;
///       </item>
///       <item>
///          Invoke <see cref="IIterator{T}.Current" />, then invoke it again before <see cref="IIterator{T}.MoveNext()" /> is invoked;
///       </item>
///       <item>
///          Invoke <see cref="IIterator{T}.MoveNext()" />, then invoke it again before <see cref="IIterator{T}.Current" /> is invoked;
///       </item>
///       <item>
///          Invoke <see cref="IIterator{T}.Current" /> or <see cref="IIterator{T}.MoveNext()" /> after the collection is modified
///          (if the sequence is mutable collection).
///       </item>
///    </list>
///    These conditions of what you must not do with iterator helps to reduce amount of fields and conditional statements in implementations.
/// </remarks>
static class IteratorRemarks { }

/// <summary>Supports a simple iteration over a sequence.</summary>
/// <typeparam name="T">The type of elements of a sequence.</typeparam>
[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IIterator<T> {
   /// <summary>
   ///    Gets the element of the sequence at the current position of the iterator.
   /// </summary>
   /// <returns>The element of the sequence at the current position of the iterator.</returns>
   /// <inheritdoc cref="IteratorRemarks" />
   T Current { get; }

   /// <summary>
   ///    Advances the iterator to the next element of the sequence.
   /// </summary>
   /// <returns>
   ///    <see langword="true" /> if the iterator was successfully advanced to the next element;
   ///    <see langword="false" /> if the iterator has passed the end of the sequence.
   /// </returns>
   /// <inheritdoc cref="IteratorRemarks" />
   bool MoveNext ();
}
