namespace Blinq.Collections;

public struct TableIterator<T>: IIterator<T> {
   TableIteratorImpl<T, T, SelectOutputOfTableIterator<T>> Impl;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal TableIterator (TableIteratorImpl<T, T, SelectOutputOfTableIterator<T>> impl) {
      Impl = impl;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      return Impl.TryPop(out item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      return Impl.Fold(accumulator, fold);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Impl.TryGetCount(out count);
   }
}
