namespace Blinq;

readonly struct FilterByTypeFold<TFrom, TAccumulator, TTo, TInnerFold>: IFold<TFrom, TAccumulator>
where TInnerFold: IFold<TTo, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterByTypeFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref TAccumulator accumulator) {
      return item is TTo outItem && InnerFold.Invoke(outItem, ref accumulator);
   }
}

readonly struct FilterByTypePopFold<TFrom, TTo>: IFold<TFrom, Option<TTo>> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref Option<TTo> accumulator) {
      if (item is TTo outItem) {
         accumulator = Option.Value(outItem);
         return true;
      } else {
         return false;
      }
   }
}

public struct FilterByTypeIterator<TTo, TFrom, TFromIterator>: IIterator<TTo>
where TTo: TFrom
where TFromIterator: IIterator<TFrom> {
   TFromIterator FromIterator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterByTypeIterator (TFromIterator fromIterator) {
      FromIterator = fromIterator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TTo item) {
      var result = FromIterator.Fold(Option<TTo>.None, new FilterByTypePopFold<TFrom, TTo>());
      return result.Is(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TTo, TAccumulator> {
      return FromIterator.Fold(accumulator, new FilterByTypeFold<TFrom, TAccumulator, TTo, TFold>(fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = default;
      return false;
   }
}

public readonly partial struct FilterContinuation<T, TIterator> {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Contract<IIterator<TTo>, FilterByTypeIterator<TTo, T, TIterator>> ByType<TTo> () where TTo: T {
      return new FilterByTypeIterator<TTo, T, TIterator>(Iterator);
   }
}
