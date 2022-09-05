namespace Blinq;

readonly struct FilterFoldFunc<TIn, TAccumulator, TOut, TSelector, TInnerFoldFunc>: IFoldFunc<TIn, TAccumulator>
where TSelector: ISelector<TIn, Option<TOut>>
where TInnerFoldFunc: IFoldFunc<TOut, TAccumulator> {
   readonly TSelector Selector;
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterFoldFunc (TSelector selector, TInnerFoldFunc innerFoldFunc) {
      Selector = selector;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref TAccumulator accumulator) {
      return Selector.Invoke(item).Is(out var outItem) && InnerFoldFunc.Invoke(outItem, ref accumulator);
   }
}

public struct FilterIterator<TOut, TIn, TSelector, TInIterator>: IIterator<TOut>
where TSelector: ISelector<TIn, Option<TOut>>
where TInIterator: IIterator<TIn> {
   TInIterator InIterator;
   readonly TSelector Selector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterIterator (TInIterator inIterator, TSelector selector) {
      InIterator = inIterator;
      Selector = selector;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      return InIterator.Fold(seed, new FilterFoldFunc<TIn, TAccumulator, TOut, TSelector, TFoldFunc>(Selector, func));
   }
}

public readonly partial struct FilterContinuation<T, TIterator> where TIterator: IIterator<T> {
   readonly Sequence<T, TIterator> Sequence;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterContinuation (Sequence<T, TIterator> sequence) {
      Sequence = sequence;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static FilterContinuation<T, TIterator> Filter<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return new FilterContinuation<T, TIterator>(sequence);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, FilterIterator<TResult, T, TSelector, TIterator>> Filter<T, TIterator, TResult, TSelector> (
      this in Sequence<T, TIterator> sequence,
      TSelector selector,
      Use<TResult> resultUse = default
   )
   where TIterator: IIterator<T>
   where TSelector: ISelector<T, Option<TResult>> {
      return new FilterIterator<TResult, T, TSelector, TIterator>(sequence.Iterator, selector);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, FilterIterator<TResult, T, FuncSelector<T, Option<TResult>>, TIterator>> Filter<T, TIterator, TResult> (
      this in Sequence<T, TIterator> sequence,
      Func<T, Option<TResult>> selector
   ) where TIterator: IIterator<T> {
      return sequence.Filter(new FuncSelector<T, Option<TResult>>(selector), Use<TResult>.Here);
   }
}
