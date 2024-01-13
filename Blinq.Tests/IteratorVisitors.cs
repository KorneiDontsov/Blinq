using System.Collections.Generic;

namespace Blinq.Tests;

readonly struct AllItemsToListVisitor<T>
   : IIteratorVisitor<T, List<T>> {
   public bool Visit (ref List<T> state, in T item) {
      state.Add(item);
      return false;
   }
}

readonly struct OneItemToListVisitor<T>
   : IIteratorVisitor<T, (List<T> list, bool isVisited)> {
   public bool Visit (ref (List<T> list, bool isVisited) state, in T item) {
      state.isVisited = true;
      state.list.Add(item);
      return true;
   }
}

readonly struct TwoItemsToListVisitor<T>
   : IIteratorVisitor<T, (List<T> list, bool isVisited, bool isSecondItem)> {
   public bool Visit (ref (List<T> list, bool isVisited, bool isSecondItem) state, in T item) {
      state.isVisited = true;
      state.list.Add(item);

      var isSecondItem = state.isSecondItem;
      state.isSecondItem = !isSecondItem;
      return isSecondItem;
   }
}
