using NUnit.Framework;

namespace Blinq.Tests;

public abstract class ArrayIteratorTests<T>: IteratorTestsTemplate<T, ArrayIterator<T>> {
   private protected ArrayIteratorTests (IArrayFactory<T> arrayFactory): base(arrayFactory) { }

   protected override ArrayIterator<T> ToIterator (T[] array) {
      return array.Iterate();
   }
}

[TestFixture]
public class ObjectArrayIteratorTests: ArrayIteratorTests<object> {
   public ObjectArrayIteratorTests (): base(ObjectArrayFactory.shared) { }
}

[TestFixture]
public class Int64ArrayIteratorTests: ArrayIteratorTests<long> {
   public Int64ArrayIteratorTests (): base(Int64ArrayFactory.shared) { }
}
