using NUnit.Framework;

namespace Blinq.Tests;

[TestFixture]
public class OptionTests {
   static void AssertIsNone (Option<object> option) {
      Assert.That(option.hasValue, Is.False);
   }

   static void AssertAreSame (Option<object> option, object obj) {
      Assert.Multiple(
         () => {
            Assert.That(option.hasValue, Is.True);
            Assert.That(option.Value(), Is.SameAs(obj));
         }
      );
   }

   [Test]
   public void TypedNoneReturnsNone () {
      var option = Option<object>.none;
      AssertIsNone(option);
   }

   [Test]
   public void ValueConvertsToOption () {
      var obj = new object();
      Option<object> option = obj;
      AssertAreSame(option, obj);
   }

   [Test]
   public void NoneOptionConvertsToTypedOption () {
      Option<object> option = Option.none;
      AssertIsNone(option);
   }

   [Test]
   public void SomeIsCreated () {
      var obj = new object();
      var option = Option.Some(obj);
      AssertAreSame(option, obj);
   }

   [Test]
   public void AccessToValueOnNoneOptionGetsException () {
      Option<object> option = default;
      Assert.Throws<AssertException>(() => _ = option.Value());
   }

   [Test]
   public void IsMethodReturnsTrueAndProvidesValueForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      var hasValue = option.Is(out var optionValue);
      Assert.Multiple(
         () => {
            Assert.That(hasValue, Is.True);
            Assert.That(optionValue, Is.SameAs(obj));
         }
      );
   }

   [Test]
   public void IsMethodReturnsFalseForNone () {
      Option<object> option = default;
      var hasValue = option.Is(out _);
      Assert.That(hasValue, Is.False);
   }

   [Test]
   public void SomeIsDeconstructed () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      option.Deconstruct(out var hasValue, out var optionValue);
      Assert.Multiple(
         () => {
            Assert.That(hasValue, Is.True);
            Assert.That(optionValue, Is.SameAs(obj));
         }
      );
   }

   [Test]
   public void NoneIsDeconstructed () {
      Option<object> option = default;
      option.Deconstruct(out var hasValue, out _);
      Assert.That(hasValue, Is.False);
   }

   [Test]
   public void OrMethodReturnsValueForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      var result = option.Or(new object());
      Assert.That(result, Is.SameAs(obj));
   }

   [Test]
   public void OrMethodReturnsElseValueForNone () {
      Option<object> option = default;
      var elseValue = new object();
      var result = option.Or(elseValue);
      Assert.That(result, Is.SameAs(elseValue));
   }

   [Test]
   public void OrDefaultMethodReturnsValueForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      var result = option.OrDefault();
      Assert.That(result, Is.SameAs(obj));
   }

   [Test]
   public void OrDefaultMethodReturnsDefaultValueForNone () {
      Option<object> option = default;
      var result = option.OrDefault();
      Assert.That(result, Is.Null);
   }

   [Test]
   public void OrElseMethodReturnsValueForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      var result = option.OrElse(() => new object());
      Assert.That(result, Is.SameAs(obj));
   }

   [Test]
   public void OrElseMethodReturnsElseValueForNone () {
      Option<object> option = default;
      var elseValue = new object();
      var result = option.OrElse(() => elseValue);
      Assert.That(result, Is.SameAs(elseValue));
   }

   [Test]
   public void OrFailMethodReturnsValueForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      var result = option.OrFail("should have value");
      Assert.That(result, Is.SameAs(obj));
   }

   [Test]
   public void OrFailMethodThrowsForNone () {
      Option<object> option = default;
      Assert.Throws<AssertException>(() => option.OrFail("expected failure"));
   }

   [Test]
   public void CoalesceMethodReturnsItselfForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };
      var result = option.Coalesce(new Option<object> { value = new object() });
      AssertAreSame(result, obj);
   }

   [Test]
   public void CoalesceMethodReturnsOtherOptionForNone () {
      Option<object> option = default;
      var elseValue = new object();
      var result = option.Coalesce(new Option<object> { value = elseValue });
      AssertAreSame(result, elseValue);
   }

   [Test]
   public void SameOptionsAreEqual () {
      var obj = new object();
      var option1 = new Option<object> { value = obj };
      var option2 = new Option<object> { value = obj };

      var areEqual = option1.Equals(option2);
      var areEqualToo = option2.Equals(option1);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.True);
            Assert.That(areEqualToo, Is.True);
         }
      );
   }

   [Test]
   public void NoneOptionsAreEqual () {
      Option<object> option1 = default;
      Option<object> option2 = default;

      var areEqual = option1.Equals(option2);
      var areEqualToo = option2.Equals(option1);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.True);
            Assert.That(areEqualToo, Is.True);
         }
      );
   }

   [Test]
   public void TwoDifferentSomeOptionsAreNotEqual () {
      var option1 = new Option<object> { value = new object() };
      var option2 = new Option<object> { value = new object() };

      var areEqual = option1.Equals(option2);
      var areEqualToo = option2.Equals(option1);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.False);
            Assert.That(areEqualToo, Is.False);
         }
      );
   }

   [Test]
   public void SomeAndNoneAreNotEqual () {
      var someOption = new Option<object> { value = new object() };
      Option<object> noneOption = default;

      var areEqual = someOption.Equals(noneOption);
      var areEqualToo = noneOption.Equals(someOption);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.False);
            Assert.That(areEqualToo, Is.False);
         }
      );
   }

   [Test]
   public void SameOptionsAreEqualNonGeneric () {
      var obj = new object();
      var option1 = new Option<object> { value = obj };
      var option2 = new Option<object> { value = obj };

      var areEqual = Equals(option1, option2);
      var areEqualToo = Equals(option2, option1);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.True);
            Assert.That(areEqualToo, Is.True);
         }
      );
   }

   [Test]
   public void NoneOptionsAreEqualNonGeneric () {
      Option<object> option1 = default;
      Option<object> option2 = default;

      var areEqual = Equals(option1, option2);
      var areEqualToo = Equals(option2, option1);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.True);
            Assert.That(areEqualToo, Is.True);
         }
      );
   }

   [Test]
   public void TwoDifferentSomeOptionsAreNotEqualNonGeneric () {
      var option1 = new Option<object> { value = new object() };
      var option2 = new Option<object> { value = new object() };

      var areEqual = Equals(option1, option2);
      var areEqualToo = Equals(option2, option1);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.False);
            Assert.That(areEqualToo, Is.False);
         }
      );
   }

   [Test]
   public void SomeAndNoneAreNotEqualNonGeneric () {
      var someOption = new Option<object> { value = new object() };
      Option<object> noneOption = default;

      var areEqual = Equals(someOption, noneOption);
      var areEqualToo = Equals(noneOption, someOption);

      Assert.Multiple(
         () => {
            Assert.That(areEqual, Is.False);
            Assert.That(areEqualToo, Is.False);
         }
      );
   }

   [Test]
   public void GetHashCodeReturnsValueHasCodeForSome () {
      var obj = new object();
      var option = new Option<object> { value = obj };

      var objHashCode = obj.GetHashCode();
      var optionHashCode = option.GetHashCode();

      Assert.That(optionHashCode, Is.EqualTo(objHashCode));
   }

   [Test]
   public void GetHashCodeReturnsZeroForNone () {
      Option<object> option = default;
      var optionHashCode = option.GetHashCode();
      Assert.That(optionHashCode, Is.EqualTo(0));
   }
}
