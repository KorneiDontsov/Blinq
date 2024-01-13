using System.Linq;
using NUnit.Framework;

namespace Blinq.Tests;

public abstract class SelectIteratorTests<TIn, TOut>
   : IteratorTests<
      TIn,
      TOut,
      SelectIterator<TOut, TIn, Functor<TIn, TOut>, ArrayIterator<TIn>>
   > {
   private protected SelectIteratorTests (IArrayFactory<TIn> arrayFactory): base(arrayFactory) { }

   protected override TOut[] GetExpectedResult (TIn[] inputArray) {
      return inputArray.Select(this.SelectOutputItem).ToArray();
   }

   protected override SelectIterator<
      TOut,
      TIn,
      Functor<TIn, TOut>,
      ArrayIterator<TIn>
   > GetActualResult (TIn[] inputArray) {
      return inputArray.Iterate().Select(this.SelectOutputItem);
   }

   protected abstract TOut SelectOutputItem (TIn input);
}

[TestFixture]
public class ObjectToHashCodeSelectIteratorTests: SelectIteratorTests<object, int> {
   public ObjectToHashCodeSelectIteratorTests (): base(ObjectArrayFactory.shared) { }

   protected override int SelectOutputItem (object input) {
      return input.GetHashCode();
   }
}

[TestFixture]
public class Int64ToInt32SelectIteratorTests: SelectIteratorTests<long, int> {
   public Int64ToInt32SelectIteratorTests (): base(Int64ArrayFactory.shared) { }

   protected override int SelectOutputItem (long input) {
      return (int)input;
   }
}
