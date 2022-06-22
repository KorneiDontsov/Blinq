namespace Blinq.Tests;

public abstract class TestIterate<TIterator> where TIterator: IIterator<int> {
   protected abstract Sequence<int, TIterator> Range (int n);

   [Test]
   public void OnEmpty () {
      var iterator = Range(0).Iterator;
      var notEmpty = iterator.MoveNext();
      Assert.False(notEmpty);
   }

   [TestCase(0)] [TestCase(10)] [TestCase(100)] [TestCase(1000)]
   public void Foreach (int n) {
      var position = 0;
      foreach (var actual in Range(n)) {
         Assert.AreEqual(actual, position);
         ++position;
      }

      Assert.AreEqual(position, n);
   }
}
