using System.Linq;

namespace Blinq.Tests;

public sealed class TestIteratorOverEnumerable: TestIterator<EnumeratorIterator<int>> {
   protected override Contract<IIterator<int>, EnumeratorIterator<int>> Range (int start, int count) {
      return Enumerable.Range(start, count).Iter();
   }
}
