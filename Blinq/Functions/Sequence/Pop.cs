using System.Diagnostics.CodeAnalysis;

namespace Blinq;

readonly struct PopFoldFunc<T>: IFoldFunc<T, Option<T>> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   [SuppressMessage("ReSharper", "RedundantAssignment")]
   public bool Invoke (T item, ref Option<T> accumulator) {
      accumulator = item;
      return true;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Pop<T, TIterator> (this ref Sequence<T, TIterator> sequence)
   where TIterator: IIterator<T> {
      var iterator = sequence.Iterator;

      var next = iterator.Fold(Option<T>.None, new PopFoldFunc<T>());

      if (next.HasValue) {
         var newCount = sequence.Count switch {
            (true, var count) => Option.Value(count - 1),
            _ => Option.None,
         };
         sequence = new Sequence<T, TIterator>(iterator, newCount);
      }

      return next;
   }
}
