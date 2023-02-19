namespace Blinq;

readonly struct AtFold<T>: IFold<T, (Option<T> result, int countLeft)> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (Option<T> result, int countLeft) accumulator) {
      if (accumulator.countLeft is 0) {
         accumulator.result = Option.Value(item);
         return true;
      } else {
         --accumulator.countLeft;
         return false;
      }
   }
}

readonly struct AtFromEndFold<T>: IFold<T, ValueTuple> {
   readonly int IndexFromEnd;
   readonly Queue<T> Queue;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AtFromEndFold (int indexFromEnd, Queue<T> queue) {
      IndexFromEnd = indexFromEnd;
      Queue = queue;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref ValueTuple accumulator) {
      if (Queue.Count == IndexFromEnd) Queue.Dequeue();
      Queue.Enqueue(item);
      return false;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static Option<T> At<T, TIterator> (ref TIterator iterator, int index) where TIterator: IIterator<T> {
      if (index < 0) Get.Throw<ArgumentOutOfRangeException>();
      return iterator.Fold((result: Option<T>.None, countLeft: index), new AtFold<T>()).result;
   }

   [Pure] [MethodImpl(MethodImplOptions.NoInlining)]
   static Option<T> AtFromEnd<T, TIterator> (ref TIterator iterator, int indexValue) where TIterator: IIterator<T> {
      if (iterator.TryPop(out var first)) {
         var queue = new Queue<T>();
         queue.Enqueue(first);

         iterator.Fold(new ValueTuple(), new AtFromEndFold<T>(indexValue, queue));

         if (queue.Count == indexValue) return queue.Dequeue();
      }

      return Option.None;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> At<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, int index) where TIterator: IIterator<T> {
      var iter = iterator.Value;
      return At<T, TIterator>(ref iter, index);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> At<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, Index index) where TIterator: IIterator<T> {
      var iter = iterator.Value;
      var indexValue = index.Value;
      if (!index.IsFromEnd) {
         return At<T, TIterator>(ref iter, indexValue);
      } else if (iter.TryGetCount(out var count)) {
         return indexValue <= count ? At<T, TIterator>(ref iter, count - indexValue) : Option.None;
      } else {
         return AtFromEnd<T, TIterator>(ref iter, indexValue);
      }
   }
}
