using Blinq.Functors;

namespace Blinq;

readonly struct FilterFold<TIn, TAccumulator, TOut, TSelector, TInnerFold>: IFold<TIn, TAccumulator>
where TSelector: ISelector<TIn, Option<TOut>>
where TInnerFold: IFold<TOut, TAccumulator> {
   readonly TSelector Selector;
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterFold (TSelector selector, TInnerFold innerFold) {
      Selector = selector;
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref TAccumulator accumulator) {
      return Selector.Invoke(item).Is(out var outItem) && InnerFold.Invoke(outItem, ref accumulator);
   }
}

readonly struct FilterPopFold<TIn, TOut, TSelector>: IFold<TIn, Option<TOut>>
where TSelector: ISelector<TIn, Option<TOut>> {
   readonly TSelector Selector;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterPopFold (TSelector selector) {
      Selector = selector;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref Option<TOut> accumulator) {
      accumulator = Selector.Invoke(item);
      return accumulator.HasValue;
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

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      var result = InIterator.Fold(Option<TOut>.None, new FilterPopFold<TIn, TOut, TSelector>(Selector));
      return result.Is(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TOut, TAccumulator> {
      return InIterator.Fold(accumulator, new FilterFold<TIn, TAccumulator, TOut, TSelector, TFold>(Selector, fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = default;
      return false;
   }
}

public readonly partial struct FilterContinuation<T, TIterator> where TIterator: IIterator<T> {
   readonly TIterator Iterator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterContinuation (TIterator iterator) {
      Iterator = iterator;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static FilterContinuation<T, TIterator> Filter<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator)
   where TIterator: IIterator<T> {
      return new FilterContinuation<T, TIterator>(iterator);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TResult>, FilterIterator<TResult, T, TSelector, TIterator>> Filter<T, TIterator, TResult, TSelector> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Contract<ISelector<T, Option<TResult>>, TSelector> selector
   )
   where TIterator: IIterator<T>
   where TSelector: ISelector<T, Option<TResult>> {
      return new FilterIterator<TResult, T, TSelector, TIterator>(iterator, selector);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TResult>, FilterIterator<TResult, T, FuncSelector<T, Option<TResult>>, TIterator>> Filter<T, TIterator, TResult> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Func<T, Option<TResult>> selector
   ) where TIterator: IIterator<T> {
      return iterator.Filter(Get<ISelector<T, Option<TResult>>>.AsContract(new FuncSelector<T, Option<TResult>>(selector)));
   }
}
