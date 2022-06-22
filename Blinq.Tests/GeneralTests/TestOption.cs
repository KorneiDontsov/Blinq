namespace Blinq.Tests;

public class TestOption {
   [Test]
   public void DefaultValue () {
      var option = default(Option<object>);
      Assert.False(option.HasValue);
   }

   [Test]
   public void NoneProperty () {
      var option = Option<object>.None;
      Assert.False(option.HasValue);
   }

   [Test]
   public void ConstructorAndDeconstructor () {
      var expected = new object();

      var option = new Option<object>(expected);

      Assert.True(option.HasValue);
      if (option is (true, var actual)) {
         Assert.AreSame(expected, actual);
      } else {
         Assert.Fail();
      }
   }

   [Test]
   public void Value () {
      var expected = new object();

      var option = Option.Value(expected);
      var actual = option.Value();

      Assert.True(option.HasValue);
      Assert.AreSame(expected, actual);
   }

   [Test]
   public void NoValue () {
      var option = Option<object>.None;
      Assert.Throws<NoValueException>(() => option.Value());
   }

   [Test]
   public void Or () {
      var expected = new object();
      var option = Option<object>.None;

      var actual = option.Or(expected);

      Assert.AreSame(expected, actual);
   }

   [Test]
   public void ValueNor () {
      var expected = new object();
      var option = Option.Value(expected);

      var actual = option.Or(new object());

      Assert.AreSame(expected, actual);
   }

   [Test]
   public void OrDefault () {
      var option = Option<object>.None;
      var actual = option.OrDefault();
      Assert.Null(actual);
   }

   [Test]
   public void ValueNorDefault () {
      var expected = new object();
      var option = Option.Value(expected);

      var actual = option.OrDefault();

      Assert.AreSame(expected, actual);
   }

   [TestCase("Message.")] [TestCase("Longer message here.")]
   public void OrFail (string message) {
      var option = Option<object>.None;
      Assert.Throws<NoValueException>(() => option.OrFail(message), message);
   }

   public void ValueNorFail () {
      var expected = new object();
      var option = Option.Value(expected);

      var actual = option.OrFail("Got no value.");

      Assert.AreSame(expected, actual);
   }
}
