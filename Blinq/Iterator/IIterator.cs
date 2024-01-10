using System.Diagnostics.CodeAnalysis;

namespace Blinq;

public interface IIteratorVisitor<T, TState> {
   /// <returns>
   ///    <list type="bullet">
   ///       <item>False - to visit the next element if it exists;</item>
   ///       <item>True - to break the visit.</item>
   ///    </list>
   /// </returns>
   bool Visit (ref TState state, in T item);
}

public interface IIterator<T> {
   bool TryPop ([MaybeNullWhen(false)] out T item);

   void Accept<TState, TVisitor> (ref TState state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<T, TState>;
}
