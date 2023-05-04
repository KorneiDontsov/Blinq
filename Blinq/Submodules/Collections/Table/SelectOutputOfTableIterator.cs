namespace Blinq.Collections;

interface ISelectOutputOfTableIterator<TEntry, TOut> {
   [Pure] static abstract TOut Invoke (in TEntry entry);
}

readonly struct SelectOutputOfTableIterator<T>: ISelectOutputOfTableIterator<T, T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Invoke (in T entry) {
      return entry;
   }
}
