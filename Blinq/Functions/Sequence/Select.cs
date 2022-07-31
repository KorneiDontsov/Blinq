namespace Blinq;

struct SelectFoldFunc<TIn, TAccumulator, TOut, TInnerFoldFunc>: IFoldFunc<TIn, TAccumulator>
where TInnerFoldFunc: IFoldFunc<TOut, TAccumulator> {
   readonly Func<TIn, TOut> Selector;
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SelectFoldFunc (Func<TIn, TOut> selector, TInnerFoldFunc innerFoldFunc) {
      Selector = selector;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref TAccumulator accumulator) {
      return InnerFoldFunc.Invoke(Selector(item), ref accumulator);
   }
}

public struct SelectIterator<TOut, TIn, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<TIn> {
   TInIterator InIterator;
   readonly Func<TIn, TOut> Selector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SelectIterator (TInIterator inIterator, Func<TIn, TOut> selector) {
      InIterator = inIterator;
      Selector = selector;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      return InIterator.Fold(seed, new SelectFoldFunc<TIn, TAccumulator, TOut, TFoldFunc>(Selector, func));
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
