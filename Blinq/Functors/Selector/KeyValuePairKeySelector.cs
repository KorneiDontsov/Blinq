namespace Blinq;

public readonly struct KeyValuePairKeySelector<TKey, TValue>: ISelector<KeyValuePair<TKey, TValue>, TKey> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TKey Invoke (KeyValuePair<TKey, TValue> item) {
      return item.Key;
   }
}
