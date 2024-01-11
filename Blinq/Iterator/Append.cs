using System.Diagnostics.CodeAnalysis;

namespace Blinq;

public struct AppendIterator<T, TImpl>: IIterator<T>
where TImpl: IIterator<T> {
   TImpl _impl;
   public required TImpl impl { init => this._impl = value; }

   readonly T _element;
   public required T element { init => this._element = value; }

   bool isEnded;

   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (this._impl.TryPop(out item)) return true;
      if (this.isEnded) return false;

      this.isEnded = true;
      item = this._element;
      return true;
   }

   public void Accept<TState, TVisitor> (ref TState state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<T, TState> {
      if (Iterator.TryBreak(ref this._impl, ref state, visitor, Pin<T>.type)) return;
      if (this.isEnded) return;

      this.isEnded = true;
      _ = visitor.Visit(ref state, in this._element);
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
