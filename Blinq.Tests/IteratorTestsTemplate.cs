using System.Collections.Generic;
using NUnit.Framework;

namespace Blinq.Tests;

public static class IteratorTestSources {
   public static IEnumerable<int> testCaseLengths { get; } =
      new[] { 0, 1, 2, 3, 4, 5, 7, 10, 25, 100 };
}

public abstract class IteratorTestsTemplate<T, TIterator>
where TIterator: IIterator<T> {
   readonly IArrayFactory<T> arrayFactory;

   private protected IteratorTestsTemplate (IArrayFactory<T> arrayFactory) {
      this.arrayFactory = arrayFactory;
   }

   protected abstract TIterator ToIterator (T[] array);

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void TryPopAll (int length) {
      var array = this.arrayFactory.GenerateArray(length);
      var iterator = this.ToIterator(array);

      var iterationResult = new List<T>();
      while (iterator.TryPop(out var item)) {
         iterationResult.Add(item);
      }

      var extraPopTry = iterator.TryPop(out _);
      Assert.Multiple(
         () => {
            Assert.That(iterationResult, Is.EqualTo(array));
            Assert.That(extraPopTry, Is.False);
         }
      );
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void AcceptAll (int length) {
      var array = this.arrayFactory.GenerateArray(length);
      var iterator = this.ToIterator(array);

      var iterationResult = new List<T>();
      iterator.Accept(ref iterationResult, new AllItemsToListVisitor<T>());
      Assert.That(iterationResult, Is.EqualTo(array));

      var extraIterationResult = new List<T>();
      iterator.Accept(ref extraIterationResult, new AllItemsToListVisitor<T>());
      Assert.That(extraIterationResult, Is.Empty);
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void AcceptAllOneByOne (int length) {
      var array = this.arrayFactory.GenerateArray(length);
      var iterator = this.ToIterator(array);

      var iterationResult = new List<T>();
      var visitorState = (list: iterationResult, isVisited: true);
      do {
         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new OneItemToListVisitor<T>());
      } while (visitorState.isVisited);

      Assert.That(iterationResult, Is.EqualTo(array));
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void AcceptAllTwoByTwo (int length) {
      var array = this.arrayFactory.GenerateArray(length);
      var iterator = this.ToIterator(array);

      var iterationResult = new List<T>();
      var visitorState = (list: iterationResult, isVisited: true, isSecondItem: false);
      do {
         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new TwoItemsToListVisitor<T>());
      } while (visitorState.isVisited);

      Assert.That(iterationResult, Is.EqualTo(array));
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void RotateToTryPopAndAcceptOneByOne (int length) {
      var array = this.arrayFactory.GenerateArray(length);
      var iterator = this.ToIterator(array);

      var iterationResult = new List<T>();
      var visitorState = (list: iterationResult, isVisited: true);
      while (visitorState.isVisited && iterator.TryPop(out var item)) {
         iterationResult.Add(item);

         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new OneItemToListVisitor<T>());
      }

      Assert.That(iterationResult, Is.EqualTo(array));
   }

   [TestCaseSource(typeof(IteratorTestSources), nameof(IteratorTestSources.testCaseLengths))]
   public void RotateToTryPopAndAcceptTwoByTwo (int length) {
      var array = this.arrayFactory.GenerateArray(length);
      var iterator = this.ToIterator(array);

      var iterationResult = new List<T>();
      var visitorState = (list: iterationResult, isVisited: true, isSecondItem: false);
      while (visitorState.isVisited && iterator.TryPop(out var item)) {
         iterationResult.Add(item);

         visitorState.isVisited = false;
         iterator.Accept(ref visitorState, new TwoItemsToListVisitor<T>());
      }

      Assert.That(iterationResult, Is.EqualTo(array));
   }
}
