using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct UpcastVisitor<TIn, TOut, TState, TImpl>: IIteratorVisitor<TIn, TState>
where TImpl: IIteratorVisitor<TOut, TState> {
   public required TImpl impl { get; init; }

   public bool Visit (ref TState state, in TIn item) {
      ref readonly var outItem = ref Unsafe.As<TIn, TOut>(ref Unsafe.AsRef(in item));
      impl.Visit(ref state, in outItem);
      return false;
   }
}

public struct UpcastIterator<TOut, TIn, TImpl>: IIterator<TOut>
where TOut: class
where TIn: TOut
where TImpl: IIterator<TIn> {
   TImpl _impl;
   public required TImpl impl { init => _impl = value; }

   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      Unsafe.SkipInit(out item);
      return _impl.TryPop(out Unsafe.As<TOut?, TIn?>(ref item));
   }

   public void Accept<TState, TVisitor> (ref TState state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<TOut, TState> {
      _impl.Accept(
         ref state,
         new UpcastVisitor<TIn, TOut, TState, TVisitor> { impl = visitor }
      );
   }
}

public static partial class Iterator {
   public static Pin<
      IIterator<TResult>,
      UpcastIterator<TResult, T, TIterator>
   > Upcast<T, TIterator, TResult> (
      this Pin<IIterator<T>, TIterator> iterator,
      TypePin<TResult> tResult = default
   )
   where T: TResult
   where TResult: class
   where TIterator: IIterator<T> {
      _ = tResult;
      return new() { value = new() { impl = iterator } };
   }
}
