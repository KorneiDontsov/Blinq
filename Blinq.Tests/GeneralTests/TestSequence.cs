using System.Collections.Generic;
using System.Linq;

namespace Blinq.Tests;

public class TestSequence {
   static Sequence<int, EnumeratorIterator<int>> Range (int n) {
      return Enumerable.Range(0, n).Iterate();
   }

   static Sequence<int, EnumeratorIterator<int>> Range (int start, int count) {
      return Enumerable.Range(start, count).Iterate();
   }

   [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void Aggregate (int n) {
      var expected = Enumerable.Range(0, n).Sum();
      var actual = Range(n).Aggregate((a, b) => a + b);
      Assert.True(actual.HasValue);
      Assert.AreEqual(expected, actual.Value());
   }

   [Test]
   public void AggregateOnEmpty () {
      var actual = Range(0).Aggregate((a, b) => a + b);
      Assert.False(actual.HasValue);
   }

   [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void AggregateWithSeed (int n) {
      const long seed = 1;
      var expected = seed + Enumerable.Range(0, n).Sum();
      var actual = Range(n).Aggregate(seed, (acc, item) => acc + item);
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void AggregateOnEmptyWithSeed () {
      var expected = new object();
      var actual = Range(0).Aggregate(expected, (_, _) => new object());
      Assert.AreSame(expected, actual);
   }

   [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void AggregateTo (int n) {
      var expected = Enumerable.Range(0, n).Aggregate(
         new List<int>(),
         (list, item) => {
            list.Add(item);
            return list;
         }
      );
      var actual = Range(n).AggregateTo(new List<int>(), (list, item) => list.Add(item));
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void AggregateToOnEmpty () {
      var expected = new object();
      var actual = Range(0).AggregateTo(expected, (_, _) => Assert.Fail());
      Assert.AreSame(expected, actual);
   }

   [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void AsEnumerable (int n) {
      var expected = Enumerable.Range(0, n);
      var actual = Range(n).AsEnumerable();

      Assert.AreEqual(expected, actual);
   }

   [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void Capture (int n) {
      var capture = new object();
      var expected = 0;
      foreach (var item in Range(n).Capture(capture)) {
         Assert.AreEqual(expected, item.Value);
         Assert.AreSame(capture, item.Capture);
         ++expected;
      }

      Assert.AreEqual(n, expected);
   }

   [Test]
   public void Empty () {
      var iterator = Sequence.Empty<object>().Iterator;
      var isNotEmpty = iterator.MoveNext();
      Assert.False(isNotEmpty);
   }

   [Test]
   public void FlattenOnEmpty () {
      var sequence = Sequence.Empty<Sequence<ValueTuple, IIterator<ValueTuple>>>();
      var actual = sequence.Flatten().AsEnumerable();
      Assert.IsEmpty(actual);
   }

   [Test]
   public void FlattenOnSingleItem () {
      var item = new object();
      var sequence = new[] { new[] { item } }.Iterate().Select(nested => nested.Iterate());

      var expected = new[] { item }.AsEnumerable();
      var actual = sequence.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnMultipleItems () {
      var items = new object[] { new(), new(), new(), new(), new(), new() };
      var sequence = new[] {
            new[] { items[0] },
            new[] { items[1], items[2] },
            new[] { items[3], items[4], items[5] },
         }
         .Iterate().Select(nested => nested.Iterate());

      var expected = items.AsEnumerable();
      var actual = sequence.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnBoxedItems () {
      var sequence = new[] {
            Sequence.Empty<int>().Box(),
            new[] { 1, 2 }.Iterate().Box(),
            Enumerable.Range(3, 3).Iterate().Box(),
         }
         .Iterate();

      var expected = new[] { 1, 2, 3, 4, 5 }.AsEnumerable();
      var actual = sequence.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0, 0)] [TestCase(10, 2)] [TestCase(100, 3)]
   public void Numerate (int n, int offset) {
      var expectedValue = offset;
      var expectedPosition = 0;

      var sequence = Range(offset, n).Numerate();

      foreach (var (actualValue, actualPosition) in sequence) {
         Assert.AreEqual(expectedValue, actualValue);
         Assert.AreEqual(expectedPosition, actualPosition);

         ++expectedValue;
         ++expectedPosition;
      }

      Assert.AreEqual(n, expectedPosition);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void DropNumeration (int n) {
      var expected = Enumerable.Range(0, n);
      var actual = expected.Iterate().Numerate().DropNumeration().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0, 0)] [TestCase(10, 2)] [TestCase(100, 3)]
   public void Select (int n, int offset) {
      var expected = Enumerable.Range(offset, n);
      var actual = Range(n).Select(i => i + offset).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0, 0)] [TestCase(10, 2)] [TestCase(100, 3)]
   public void Where (int n, int basis) {
      bool Predicate (int i) {
         return i % basis == 0;
      }

      var expected = Enumerable.Range(0, n).Where(Predicate);
      var actual = Range(n).Where(Predicate).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }
}
