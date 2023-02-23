using System.Linq;

namespace Blinq.Tests;

public sealed class TestIteratorOverArray: TestIterator<ArrayIterator<int>> {
   protected override Contract<IIterator<int>, ArrayIterator<int>> Range (int start, int count) {
      return Enumerable.Range(start, count).ToArray().Iterate();
   }
}
