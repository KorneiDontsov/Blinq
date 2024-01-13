using System;
using System.Linq;
using NUnit.Framework;

namespace Blinq.Tests;

using Assert = NUnit.Framework.Assert;

[TestFixture]
public class Test1 {
   [Test]
   public void SelectSum () {
      var array = new[] {
         0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
         10946, 17711,
      };
      var linqSum = array.Select(number => (long)number * number).Sum();
      var blinqSum = array.Iterate().Select(number => (long)number * number).Sum();

      Assert.That(blinqSum, Is.EqualTo(507544127L));
      Assert.That(blinqSum, Is.EqualTo(linqSum));
   }

   [Test]
   public void SelectAggregate () {
      var array = new Int128[] {
         0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765,
         10946, 17711,
      };
      var linqSum =
         array.Select(static number => number * number)
            .Aggregate(Int128.Zero, (a, b) => a + b);
      var blinqSum =
         array.Iterate()
            .Select(static number => number * number)
            .Aggregate(Int128.Zero, (a, b) => a + b);

      Assert.That(blinqSum, Is.EqualTo((Int128)507544127));
      Assert.That(blinqSum, Is.EqualTo(linqSum));
   }
}
