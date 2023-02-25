using Blinq.Functors;

namespace Blinq;

readonly struct SelectFold<TIn, TAccumulator, TOut, TSelector, TInnerFold>: IFold<TIn, TAccumulator>
where TSelector: ISelector<TIn, TOut>
where TInnerFold: IFold<TOut, TAccumulator> {
   readonly TSelector Selector;
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SelectFold (TSelector selector, TInnerFold innerFold) {
      Selector = selector;
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn item, ref TAccumulator accumulator) {
      return InnerFold.Invoke(Selector.Invoke(item), ref accumulator);
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
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      if (InIterator.TryPop(out var inputItem)) {
         item = Selector.Invoke(inputItem);
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TOut, TAccumulator> {
      return InIterator.Fold(accumulator, new SelectFold<TIn, TAccumulator, TOut, TSelector, TFold>(Selector, fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return InIterator.TryGetCount(out count);
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TResult>, SelectIterator<TResult, T, TSelector, TIterator>> Select<T, TIterator, TResult, TSelector> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Contract<ISelector<T, TResult>, TSelector> selector
   )
   where TIterator: IIterator<T>
   where TSelector: ISelector<T, TResult> {
      return new SelectIterator<TResult, T, TSelector, TIterator>(iterator, selector);
   }

   /// <summary>Projects each element of a sequence into a new form.</summary>
   /// <param name="selector">A transform function to apply to each element.</param>
   /// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
   /// <returns>A sequence whose elements are the result of invoking the transform function on each element of <paramref name="iterator" />.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TResult>, SelectIterator<TResult, T, FuncSelector<T, TResult>, TIterator>> Select<T, TIterator, TResult> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Func<T, TResult> selector
   )
   where TIterator: IIterator<T> {
      return iterator.Select(Get<ISelector<T, TResult>>.AsContract(new FuncSelector<T, TResult>(selector)));
   }
}
