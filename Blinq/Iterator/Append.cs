using System.Diagnostics.CodeAnalysis;

namespace Blinq;

public struct AppendIterator<T, TImpl>: IIterator<T>
where TImpl: IIterator<T> {
   TImpl _impl;
   public required TImpl impl { init => _impl = value; }

   readonly T _element;
   public required T element { init => _element = value; }

   bool isEnded;

   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (_impl.TryPop(out item)) return true;
      if (isEnded) return false;

      isEnded = true;
      item = _element;
      return true;
   }

   public void Accept<TState, TVisitor> (ref TState state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<T, TState> {
      if (Iterator.TryBreak(ref _impl, ref state, visitor, Pin<T>.type)) return;
      if (isEnded) return;

      isEnded = true;
      _ = visitor.Visit(ref state, in _element);
   }
}

public static partial class Iterator {
   public static Pin<IIterator<T>, AppendIterator<T, TIterator>> Append<T, TIterator> (
      this Pin<IIterator<T>, TIterator> iterator,
      T element
   ) where TIterator: IIterator<T> {
      return new() { value = new() { impl = iterator, element = element } };
   }
}
