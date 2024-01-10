using System.Diagnostics.CodeAnalysis;

namespace Blinq;

public struct PrependIterator<T, TImpl>: IIterator<T>
where TImpl: IIterator<T> {
   TImpl _impl;
   public required TImpl impl { init => _impl = value; }

   readonly T _element;
   public required T element { init => _element = value; }

   bool isStarted;

   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (!isStarted) {
         isStarted = true;
         item = _element;
         return true;
      }

      return _impl.TryPop(out item);
   }

   public void Accept<TState, TVisitor> (ref TState state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<T, TState> {
      if (!isStarted) {
         isStarted = true;
         if (visitor.Visit(ref state, in _element)) return;
      }

      _impl.Accept(ref state, visitor);
   }
}

public static partial class Iterator {
   public static Pin<IIterator<T>, PrependIterator<T, TIterator>> Prepend<T, TIterator> (
      this Pin<IIterator<T>, TIterator> iterator,
      T element
   ) where TIterator: IIterator<T> {
      return new() { value = new() { impl = iterator, element = element } };
   }
}
