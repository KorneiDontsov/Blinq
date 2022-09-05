namespace Blinq;

readonly struct AtFoldFunc<T>: IFoldFunc<T, (Option<T> result, int countLeft)> {
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

readonly struct AtFromEndFoldFunc<T>: IFoldFunc<T, ValueTuple> {
   readonly int IndexFromEnd;
   readonly Queue<T> Queue;

   public AtFromEndFoldFunc (int indexFromEnd, Queue<T> queue) {
      IndexFromEnd = indexFromEnd;
      Queue = queue;
   }

   public bool Invoke (T item, ref ValueTuple accumulator) {
      if (Queue.Count == IndexFromEnd) Queue.Dequeue();
      Queue.Enqueue(item);
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> At<T, TIterator> (this in Sequence<T, TIterator> sequence, int index) where TIterator: IIterator<T> {
      if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, null);
      return sequence.Iterator.Fold((result: Option<T>.None, countLeft: index), new AtFoldFunc<T>()).result;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> At<T, TIterator> (this in Sequence<T, TIterator> sequence, Index index) where TIterator: IIterator<T> {
      var indexValue = index.Value;
      if (!index.IsFromEnd) {
         return sequence.At(indexValue);
      } else if (sequence.Count is (true, var count)) {
         return sequence.At(count - indexValue);
      } else {
         var iterator = sequence.Iterator;
         if (Sequence<T>.Pop(ref iterator) is (true, var first)) {
            var queue = new Queue<T>();
            queue.Enqueue(first);

            iterator.Fold(new ValueTuple(), new AtFromEndFoldFunc<T>(indexValue, queue));

            if (queue.Count == indexValue) return queue.Dequeue();
         }

         return Option.None;
      }
   }
}
