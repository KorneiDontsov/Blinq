using System.Linq;

namespace Blinq.Tests;

public sealed class TestSequenceOverEnumerable: TestSequence<EnumeratorIterator<int>> {
   protected override Sequence<int, EnumeratorIterator<int>> Range (int start, int count) {
      return Enumerable.Range(start, count).Iterate();
   }
}
