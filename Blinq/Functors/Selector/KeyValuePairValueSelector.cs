using System.Collections.Generic;

namespace Blinq;

public readonly struct KeyValuePairValueSelector<TKey, TValue>: ISelector<KeyValuePair<TKey, TValue>, TValue> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TValue Invoke (KeyValuePair<TKey, TValue> item) {
      return item.Value;
   }
}
