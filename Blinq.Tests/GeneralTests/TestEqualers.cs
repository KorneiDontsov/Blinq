namespace Blinq.Tests;

public class TestEqualers {
   record Point(decimal X, decimal Y);

   [Test]
   public void AreEqualByDefault () {
      var a = new Point(1, 2);
      var b = new Point(1, 2);
      var areEqual = a.Equals(b, e => e.Default());
      Assert.True(areEqual);
   }

   [Test]
   public void NotEqualByDefault () {
      var a = new Point(1, 2);
      var b = new Point(1, 4);
      var areEqual = a.Equals(b, e => e.Default());
      Assert.False(areEqual);
   }

   [Test]
   public void AreEqualByImpl () {
      var a = new Point(1, 2);
      var b = new Point(1, 2);
      var areEqual = a.Equals(b, e => e.ByImpl());
      Assert.True(areEqual);
   }

   [Test]
   public void NotEqualByImpl () {
      var a = new Point(1, 2);
      var b = new Point(1, 4);
      var areEqual = a.Equals(b, e => e.ByImpl());
      Assert.False(areEqual);
   }

   [Test]
   public void AreEqualByRef () {
      var a = new Point(1, 2);
      var b = a;
      var areEqual = a.Equals(b, e => e.ByRef());
      Assert.True(areEqual);
   }

   [Test]
   public void NotEqualByRef () {
      var a = new Point(1, 2);
      var b = new Point(1, 2);
      var areEqual = a.Equals(b, e => e.ByRef());
      Assert.False(areEqual);
   }

   [Test]
   public void AreEqualByKey () {
      var a = new Point(1, 2);
      var b = new Point(1, 4);
      var areEqual = a.Equals(b, e => e.ByKey(p => p.X));
      Assert.True(areEqual);
   }

   [Test]
   public void NotEqualByKey () {
      var a = new Point(1, 2);
      var b = new Point(2, 4);
      var areEqual = a.Equals(b, e => e.ByKey(p => p.X));
      Assert.False(areEqual);
   }

   class PointContainer {
      public Point Point { get; }

      public PointContainer (Point point) {
         Point = point;
      }
   }

   [Test]
   public void AreEqualByKeyByRef () {
      var point = new Point(1, 2);
      var a = new PointContainer(point);
      var b = new PointContainer(point);
      var areEqual = a.Equals(b, e => e.ByKey(c => c.Point, ke => ke.ByRef()));
      Assert.True(areEqual);
   }

   [Test]
   public void NotEqualByKeyByRef () {
      var a = new PointContainer(new Point(1, 2));
      var b = new PointContainer(new Point(1, 2));
      var areEqual = a.Equals(b, e => e.ByKey(c => c.Point, ke => ke.ByRef()));
      Assert.False(areEqual);
   }
}
