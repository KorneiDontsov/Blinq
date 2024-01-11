using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct SelectVisitor<TIn, TState, TOut, TSelector, TImpl>:
   IIteratorVisitor<TIn, TState>
where TSelector: IFunctor<TIn, TOut>
where TImpl: IIteratorVisitor<TOut, TState> {
   public required TImpl impl { get; init; }
   public required TSelector selector { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Visit (ref TState state, in TIn item) {
      return this.impl.Visit(ref state, this.selector.Invoke(in item));
   }
}

public struct SelectIterator<TOut, TIn, TSelector, TImpl>: IIterator<TOut>
where TSelector: IFunctor<TIn, TOut>
where TImpl: IIterator<TIn> {
   TImpl _impl;
   public required TImpl impl { init => this._impl = value; }

   readonly TSelector _selector;
   public required TSelector selector { init => this._selector = value; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      if (this._impl.TryPop(out var itemProto)) {
         item = this._selector.Invoke(itemProto);
         return true;
      }

      Advanced.SkipInit(out item);
      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Accept<TState, TVisitor> (ref TState state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<TOut, TState> {
      this._impl.Accept(
         ref state,
         new SelectVisitor<TIn, TState, TOut, TSelector, TVisitor> {
            impl = visitor,
            selector = this._selector,
         }
      );
   }
}

public static partial class Iterator {
   public static Pin<
      IIterator<TResult>,
      SelectIterator<TResult, T, TSelector, TIterator>
   > Select<T, TIterator, TResult, TSelector> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<IFunctor<T, TResult>, TSelector> selector
   )
   where TIterator: IIterator<T>
   where TSelector: IFunctor<T, TResult> {
      return new() { value = new() { impl = iterator, selector = selector } };
   }
}
