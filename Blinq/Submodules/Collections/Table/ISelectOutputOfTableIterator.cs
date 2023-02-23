namespace Blinq.Collections;

interface ISelectOutputOfTableIterator<TEntry, TOut> {
   [Pure] static abstract TOut Invoke (in TEntry entry);
}

interface ISelectOutputOfTableIterator<T>: ISelectOutputOfTableIterator<T, T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static T ISelectOutputOfTableIterator<T, T>.Invoke (in T entry) {
      return entry;
   }
}
