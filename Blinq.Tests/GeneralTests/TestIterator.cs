using System.Linq;

namespace Blinq.Tests;

public class TestIterator {
   [Test]
   public void Empty () {
      var expected = Enumerable.Empty<object>();
      var actual = Iterator.Empty<object>().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnEmpty () {
      var iterator = Iterator.Empty<Contract<IIterator<ValueTuple>, IIterator<ValueTuple>>>();
      var actual = iterator.Flatten().AsEnumerable();
      Assert.IsEmpty(actual);
   }
}

public abstract class TestIterator<TIterator> where TIterator: IIterator<int> {
   protected abstract Contract<IIterator<int>, TIterator> Range (int start, int count);

   Contract<IIterator<int>, TIterator> Range (int n) {
      return Range(0, n);
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

   [Test]
   public void AppendOnEmpty () {
      var expected = new[] { 1 }.AsEnumerable();
      var actual = Range(0).Append(1).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void Append () {
      var expected = new[] { 0, 1, 2, 3, 4, 100 }.AsEnumerable();
      var actual = Range(5).Append(100).AsEnumerable();
      Assert.AreEqual(expected, actual);
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

      var iterator = Range(n).Capture(capture);
      while (iterator.Pop().Is(out var item)) {
         Assert.AreEqual(expected, item.Value);
         Assert.AreSame(capture, item.Capture);
         ++expected;
      }

      Assert.AreEqual(n, expected);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void Count (int n) {
      var expected = n;
      var actual = Range(n).Count();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnEmptyItem () {
      var expected = Enumerable.Empty<int>();
      var iterator = new[] { Range(0) }.Iter();
      var actual = iterator.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnSingleItem () {
      var expected = new[] { 0 }.AsEnumerable();
      var iterator = new[] { Range(1) }.Iter();
      var actual = iterator.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnMultipleItems () {
      var expected = new[] { 0, 1, 0, 1, 2, 0, 1, 2, 3, 4 }.AsEnumerable();
      var iterator = new[] { Range(2), Range(3), Range(5) }.Iter();
      var actual = iterator.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnMultipleItemsWithSingleFold () {
      var expected = 14;
      var iterator = new[] { Range(2), Range(3), Range(5) }.Iter();
      var actual = iterator.Flatten().Sum();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void FlattenOnBoxedItems () {
      var expected = new[] { 0, 1, 0, 1, 2, 0, 1, 2, 3, 4 }.AsEnumerable();
      var iterator = new[] { Range(2).Box(), Range(3).Box(), Range(5).Box() }.Iter();
      var actual = iterator.Flatten().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)]
   public void ForEach (int n) {
      var expected = Enumerable.Range(0, n);

      var actual = new List<int>();
      Range(n).ForEach(actual.Add);

      Assert.AreEqual(expected, actual);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)]
   public void Inspect (int n) {
      var expected = Enumerable.Range(0, n);

      var actual = new List<int>();
      var iterator = Range(n).Inspect(actual.Add);
      Assert.IsEmpty(actual);

      while (iterator.Pop().HasValue) { }

      Assert.AreEqual(expected, actual);
   }

   [TestCase(0, 0)] [TestCase(10, 2)] [TestCase(100, 3)]
   public void Numerate (int n, int offset) {
      var expectedValue = offset;
      var expectedPosition = 0;

      var iterator = Range(offset, n).Numerate();

      while (iterator.Pop() is (true, var (actualValue, actualPosition))) {
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
      var actual = Range(n).Numerate().DropNumeration().AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void PrependOnEmpty () {
      var expected = new[] { 1 }.AsEnumerable();
      var actual = Range(0).Prepend(1).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void Prepend () {
      var expected = new[] { 100, 0, 1, 2, 3, 4 }.AsEnumerable();
      var actual = Range(5).Prepend(100).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0, 0)] [TestCase(10, 2)] [TestCase(100, 3)]
   public void Select (int n, int offset) {
      var expected = Enumerable.Range(offset, n);
      var actual = Range(n).Select(i => i + offset).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void Sum (int n) {
      var expected = n / 2 * (n - 1);
      var actual = Range(n).Sum();
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

   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void WhereCount (int n) {
      var expected = n / 2;
      var actual = Range(n).Where(i => i % 2 == 0).Count();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void WhereCompares () {
      var expected = new[] { 0, 1, 2, 3, 4 };
      var actual = Range(10).WhereCompares(5, CompareCondition.Less).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void WhereEqualToSingleResult () {
      var expected = new[] { 7 };
      var actual = Range(10).WhereEqual(7).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void WhereEqualToMultipleResults () {
      var expected = new[] { 0, 2, 4, 6, 8 };
      var actual = Range(10).WhereEqual(0, e => e.ByKey(i => i % 2)).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [Test]
   public void WhereNotEqual () {
      var expected = new[] { 1, 2, 3, 4 };
      var actual = Range(5).WhereNotEqual(0).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(10)] [TestCase(100)] [TestCase(250)]
   public void ConcatValues (int n) {
      var expected = Enumerable.Range(0, n).Concat(Enumerable.Range(0, n));
      var actual = Range(n).Concat(Range(n)).AsEnumerable();
      Assert.AreEqual(expected, actual);
   }

   [TestCase(10, 17)] [TestCase(100, 177)] [TestCase(250, 227)]
   public void ConcatCount (int n1, int n2) {
      var actual = Range(n1).Concat(Range(n2));
      Assert.AreEqual(n1 + n2, actual.Count());
   }
}
