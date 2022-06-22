using System.Linq;
using Blinq.Functions.Iterable;

namespace Blinq.Tests;

public class TestIterable {
   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void Creates (int n) {
      var expected = Enumerable.Range(0, n);
      var actual = Iterable.Create(() => expected.Iterate()).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void CreatesWithCapture (int n) {
      var expected = Enumerable.Range(0, n);
      var actual = Iterable.Create(expected, static e => e.Iterate()).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }
}
