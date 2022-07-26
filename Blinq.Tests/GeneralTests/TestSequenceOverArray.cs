using System.Linq;

namespace Blinq.Tests;

public sealed class TestSequenceOverArray: TestSequence<ArrayIterator<int>> {
   protected override Sequence<int, ArrayIterator<int>> Range (int start, int count) {
      return Enumerable.Range(start, count).ToArray().Iterate();
   }
}
