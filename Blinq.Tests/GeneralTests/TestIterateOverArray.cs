using System.Linq;

namespace Blinq.Tests;

public sealed class TestIterateOverArray: TestIterate<ArrayIterator<int>> {
   protected override Sequence<int, ArrayIterator<int>> Range (int n) {
      return Enumerable.Range(0, n).ToArray().Iterate();
   }
}
