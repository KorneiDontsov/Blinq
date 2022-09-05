namespace Blinq;

readonly struct SelectFoldFunc<TIn, TAccumulator, TOut, TSelector, TInnerFoldFunc>: IFoldFunc<TIn, TAccumulator>
where TSelector: ISelector<TIn, TOut>
where TInnerFoldFunc: IFoldFunc<TOut, TAccumulator> {
   readonly TSelector Selector;
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SelectFoldFunc (TSelector selector, TInnerFoldFunc innerFoldFunc) {
      Selector = selector;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref TAccumulator accumulator) {
      return InnerFoldFunc.Invoke(Selector.Invoke(item), ref accumulator);
   }
}

public struct SelectIterator<TOut, TIn, TSelector, TInIterator>: IIterator<TOut>
where TSelector: ISelector<TIn, TOut>
where TInIterator: IIterator<TIn> {
   TInIterator InIterator;
   readonly TSelector Selector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SelectIterator (TInIterator inIterator, TSelector selector) {
      InIterator = inIterator;
      Selector = selector;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      return InIterator.Fold(seed, new SelectFoldFunc<TIn, TAccumulator, TOut, TSelector, TFoldFunc>(Selector, func));
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, SelectIterator<TResult, T, TSelector, TIterator>> Select<T, TIterator, TResult, TSelector> (
      this in Sequence<T, TIterator> sequence,
      TSelector selector,
      Use<TResult> resultUse = default
   )
   where TIterator: IIterator<T>
   where TSelector: ISelector<T, TResult> {
      return Sequence<TResult>.Create(new SelectIterator<TResult, T, TSelector, TIterator>(sequence.Iterator, selector), sequence.Count);
   }

   /// <summary>Projects each element of a sequence into a new form.</summary>
   /// <param name="selector">A transform function to apply to each element.</param>
   /// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
   /// <returns>A sequence whose elements are the result of invoking the transform function on each element of <paramref name="sequence" />.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, SelectIterator<TResult, T, FuncSelector<T, TResult>, TIterator>> Select<T, TIterator, TResult> (
      this in Sequence<T, TIterator> sequence,
      Func<T, TResult> selector
   )
   where TIterator: IIterator<T> {
      return sequence.Select(new FuncSelector<T, TResult>(selector), Use<TResult>.Here);
   }
}
