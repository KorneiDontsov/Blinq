using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct NextAccumulator<T>: IAccumulator<T, Option<T>> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   [SuppressMessage("ReSharper", "RedundantAssignment")]
   public bool Invoke (T item, ref Option<T> accumulated) {
      accumulated = item;
      return true;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Next<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Iterator.Accumulate(new NextAccumulator<T>(), Option<T>.None);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Pop<T, TIterator> (this ref Sequence<T, TIterator> sequence)
   where TIterator: IIterator<T> {
      var iterator = sequence.Iterator;

      var next = iterator.Accumulate(new NextAccumulator<T>(), Option<T>.None);

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
