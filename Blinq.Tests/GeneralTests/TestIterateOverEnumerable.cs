using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blinq.Tests;

public sealed class TestIterateOverEnumerable: TestIterate<EnumeratorIterator<int>> {
   protected override Sequence<int, EnumeratorIterator<int>> Range (int n) {
      return Enumerable.Range(0, n).Iterate();
   }

   sealed class DisposingEnumerator: IEnumerator<ValueTuple> {
      public bool IsDisposed;

      public ValueTuple Current => default;

      object IEnumerator.Current => Current;

      public bool MoveNext () {
         return false;
      }

      void IEnumerator.Reset () {
         throw new NotSupportedException();
      }

      public void Dispose () {
         IsDisposed = true;
      }
   }

   sealed class DisposingEnumeratorEnumerable: IEnumerable<ValueTuple> {
      public DisposingEnumerator Enumerator { get; } = new();

      public IEnumerator<ValueTuple> GetEnumerator () {
         return Enumerator;
      }

      IEnumerator IEnumerable.GetEnumerator () {
         return GetEnumerator();
      }
   }

   void IterateAndAssertNoDispose (DisposingEnumeratorEnumerable enumerable, EnumeratorIterator<ValueTuple> iterator) {
      Assert.False(enumerable.Enumerator.IsDisposed);

      var notEmpty = iterator.MoveNext();
      Assert.False(notEmpty);
      Assert.False(enumerable.Enumerator.IsDisposed);
   }

   [Test]
   public void DoesntDispose () {
      var enumerable = new DisposingEnumeratorEnumerable();
      var iterator = enumerable.Iterate().Iterator;
      IterateAndAssertNoDispose(enumerable, iterator);
   }

   [Test]
   public void Disposes () {
      var enumerable = new DisposingEnumeratorEnumerable();
      enumerable.Iterate(query => IterateAndAssertNoDispose(enumerable, query.Iterator));
      Assert.True(enumerable.Enumerator.IsDisposed);
   }

   [Test]
   public void DisposesAndReturnsResult () {
      var expectedResult = new object();
      var enumerable = new DisposingEnumeratorEnumerable();
      var actualResult = enumerable.Iterate(
         query => {
            IterateAndAssertNoDispose(enumerable, query.Iterator);
            return expectedResult;
         }
      );
      Assert.True(enumerable.Enumerator.IsDisposed);
      Assert.AreSame(actualResult, expectedResult);
   }
}
