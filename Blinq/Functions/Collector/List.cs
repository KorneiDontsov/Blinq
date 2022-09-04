using System.Collections.Generic;

namespace Blinq;

public static partial class Collector {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<ICollector<T, List<T>, List<T>>, SimpleCollector<T, List<T>>> List<T> (this CollectorProvider<T> _) {
      return new SimpleCollector<T, List<T>>();
   }
}
