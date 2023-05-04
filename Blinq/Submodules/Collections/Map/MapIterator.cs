namespace Blinq.Collections;

public struct MapIterator<TKey, TValue>: IIterator<KeyValuePair<TKey, TValue>> where TKey: notnull {
   TableIteratorImpl<MapEntry<TKey, TValue>, KeyValuePair<TKey, TValue>, SelectOutputOfMapIterator<TKey, TValue>> Impl;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal MapIterator (TableIteratorImpl<MapEntry<TKey, TValue>, KeyValuePair<TKey, TValue>, SelectOutputOfMapIterator<TKey, TValue>> impl) {
      Impl = impl;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop (out KeyValuePair<TKey, TValue> item) {
      return Impl.TryPop(out item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<KeyValuePair<TKey, TValue>, TAccumulator> {
      return Impl.Fold(accumulator, fold);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Impl.TryGetCount(out count);
   }
}
