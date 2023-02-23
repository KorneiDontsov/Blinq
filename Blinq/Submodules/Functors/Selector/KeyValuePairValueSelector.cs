namespace Blinq.Functors;

public readonly struct KeyValuePairValueSelector<TKey, TValue>: ISelector<KeyValuePair<TKey, TValue>, TValue> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TValue Invoke (KeyValuePair<TKey, TValue> input) {
      return input.Value;
   }
}
