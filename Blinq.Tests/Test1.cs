using System.Linq;
using NUnit.Framework;

namespace Blinq.Tests;

[TestFixture]
public class Test1 {
   [Test]
   public void Test () {
      var array = new[] {
         0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
         10946, 17711,
      };
      var linqSum = array.Select(number => (long)number * number).Sum();
      var blinqSum = array.Iterate().Select(number => (long)number * number).Sum();

      NUnit.Framework.Assert.That(blinqSum, Is.EqualTo(507544127L));
      NUnit.Framework.Assert.That(blinqSum, Is.EqualTo(linqSum));
   }
}
