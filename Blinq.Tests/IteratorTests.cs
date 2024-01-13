using System.Collections.Generic;
using NUnit.Framework;

namespace Blinq.Tests;

public static class IteratorTestSources {
   public static IEnumerable<int> testCaseLengths { get; } =
      new[] { 0, 1, 2, 3, 4, 5, 7, 10, 25, 100 };
}

public abstract class IteratorTests<TIn, TOut, TIterator>
where TIterator: IIterator<TOut> {
   readonly IArrayFactory<TIn> arrayFactory;

   private protected IteratorTests (IArrayFactory<TIn> arrayFactory) {
      this.arrayFactory = arrayFactory;
   }

   protected abstract TOut[] GetExpectedResult (TIn[] inputArray);

   protected abstract TIterator GetActualResult (TIn[] inputArray);

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void TryPopAll (int length) {
      var inputArray = this.arrayFactory.GenerateArray(length);
      var expectedResult = this.GetExpectedResult(inputArray);
      var iterator = this.GetActualResult(inputArray);

      var iterationResult = new List<TOut>();
      while (iterator.TryPop(out var item)) {
         iterationResult.Add(item);
      }

      var extraPopTry = iterator.TryPop(out _);
      Assert.Multiple(
         () => {
            Assert.That(iterationResult, Is.EqualTo(expectedResult));
            Assert.That(extraPopTry, Is.False);
         }
      );
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void AcceptAll (int length) {
      var inputArray = this.arrayFactory.GenerateArray(length);
      var expectedResult = this.GetExpectedResult(inputArray);
      var iterator = this.GetActualResult(inputArray);

      var iterationResult = new List<TOut>();
      iterator.Accept(ref iterationResult, new AllItemsToListVisitor<TOut>());
      Assert.That(iterationResult, Is.EqualTo(expectedResult));

      var extraIterationResult = new List<TOut>();
      iterator.Accept(ref extraIterationResult, new AllItemsToListVisitor<TOut>());
      Assert.That(extraIterationResult, Is.Empty);
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void AcceptAllOneByOne (int length) {
      var inputArray = this.arrayFactory.GenerateArray(length);
      var expectedResult = this.GetExpectedResult(inputArray);
      var iterator = this.GetActualResult(inputArray);

      var iterationResult = new List<TOut>();
      var visitorState = (list: iterationResult, isVisited: true);
      do {
         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new OneItemToListVisitor<TOut>());
      } while (visitorState.isVisited);

      Assert.That(iterationResult, Is.EqualTo(expectedResult));
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void AcceptAllTwoByTwo (int length) {
      var inputArray = this.arrayFactory.GenerateArray(length);
      var expectedResult = this.GetExpectedResult(inputArray);
      var iterator = this.GetActualResult(inputArray);

      var iterationResult = new List<TOut>();
      var visitorState = (list: iterationResult, isVisited: true, isSecondItem: false);
      do {
         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new TwoItemsToListVisitor<TOut>());
      } while (visitorState.isVisited);

      Assert.That(iterationResult, Is.EqualTo(expectedResult));
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void RotateToTryPopAndAcceptOneByOne (int length) {
      var inputArray = this.arrayFactory.GenerateArray(length);
      var expectedResult = this.GetExpectedResult(inputArray);
      var iterator = this.GetActualResult(inputArray);

      var iterationResult = new List<TOut>();
      var visitorState = (list: iterationResult, isVisited: true);
      while (visitorState.isVisited && iterator.TryPop(out var item)) {
         iterationResult.Add(item);

         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new OneItemToListVisitor<TOut>());
      }

      Assert.That(iterationResult, Is.EqualTo(expectedResult));
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void RotateToTryPopAndAcceptTwoByTwo (int length) {
      var inputArray = this.arrayFactory.GenerateArray(length);
      var expectedResult = this.GetExpectedResult(inputArray);
      var iterator = this.GetActualResult(inputArray);

      var iterationResult = new List<TOut>();
      var visitorState = (list: iterationResult, isVisited: true, isSecondItem: false);
      while (visitorState.isVisited && iterator.TryPop(out var item)) {
         iterationResult.Add(item);

         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new TwoItemsToListVisitor<TOut>());
      }

      Assert.That(iterationResult, Is.EqualTo(expectedResult));
   }
}

public abstract class IteratorTests<T, TIterator>: IteratorTests<T, T, TIterator>
where TIterator: IIterator<T> {
   private protected IteratorTests (IArrayFactory<T> arrayFactory): base(arrayFactory) { }

   protected sealed override T[] GetExpectedResult (T[] inputArray) {
      return inputArray;
   }
}
