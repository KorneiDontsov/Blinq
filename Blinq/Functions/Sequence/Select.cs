using System.Runtime.CompilerServices;

namespace Blinq;

struct SelectAccumulator<TIn, TAccumulated, TOut, TNextAccumulator>: IAccumulator<TIn, TAccumulated>
where TNextAccumulator: IAccumulator<TOut, TAccumulated> {
   readonly Func<TIn, TOut> Selector;
   TNextAccumulator NextAccumulator;

   public SelectAccumulator (Func<TIn, TOut> selector, TNextAccumulator nextAccumulator) {
      Selector = selector;
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref TAccumulated accumulated) {
      return NextAccumulator.Invoke(Selector(item), ref accumulated);
   }
}

public struct SelectIterator<TOut, TIn, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<TIn> {
   TInIterator InIterator;
   readonly Func<TIn, TOut> Selector;

   public SelectIterator (TInIterator inIterator, Func<TIn, TOut> selector) {
      InIterator = inIterator;
      Selector = selector;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<TOut, TAccumulated> {
      return InIterator.Accumulate(new SelectAccumulator<TIn, TAccumulated, TOut, TAccumulator>(Selector, accumulator), seed);
   }
}

public static partial class Sequence {
   /// <summary>Projects each element of a sequence into a new form.</summary>
   /// <param name="selector">A transform function to apply to each element.</param>
   /// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
   /// <returns>A sequence whose elements are the result of invoking the transform function on each element of <paramref name="sequence" />.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, SelectIterator<TResult, T, TIterator>> Select<T, TIterator, TResult> (
      this in Sequence<T, TIterator> sequence,
      Func<T, TResult> selector
   )
   where TIterator: IIterator<T> {
      return new Sequence<TResult, SelectIterator<TResult, T, TIterator>>(
         new SelectIterator<TResult, T, TIterator>(sequence.Iterator, selector),
         sequence.Count
      );
   }
}
