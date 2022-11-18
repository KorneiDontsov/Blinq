namespace Blinq;

readonly struct FilterByTypeFoldFunc<TFrom, TAccumulator, TTo, TInnerFoldFunc>: IFoldFunc<TFrom, TAccumulator>
where TInnerFoldFunc: IFoldFunc<TTo, TAccumulator> {
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FilterByTypeFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TFrom item, ref TAccumulator accumulator) {
      return item is TTo outItem && InnerFoldFunc.Invoke(outItem, ref accumulator);
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

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TTo, TAccumulator> {
      return FromIterator.Fold(seed, new FilterByTypeFoldFunc<TFrom, TAccumulator, TTo, TFoldFunc>(func));
   }
}

public readonly partial struct FilterContinuation<T, TIterator> {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Sequence<TTo, FilterByTypeIterator<TTo, T, TIterator>> ByType<TTo> () where TTo: T {
      return new FilterByTypeIterator<TTo, T, TIterator>(Sequence.Iterator);
   }
}
