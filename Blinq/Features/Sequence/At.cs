namespace Blinq;

readonly struct AtFoldFunc<T>: IFoldFunc<T, (Option<T> result, int countLeft)> {
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

readonly struct AtFromEndFoldFunc<T>: IFoldFunc<T, ValueTuple> {
   readonly int IndexFromEnd;
   readonly Queue<T> Queue;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AtFromEndFoldFunc (int indexFromEnd, Queue<T> queue) {
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

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> At<T, TIterator> (this in Sequence<T, TIterator> sequence, int index) where TIterator: IIterator<T> {
      if (index < 0) Get.Throw<ArgumentOutOfRangeException>();
      return sequence.Iterator.Fold((result: Option<T>.None, countLeft: index), new AtFoldFunc<T>()).result;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> At<T, TIterator> (this in Sequence<T, TIterator> sequence, Index index) where TIterator: IIterator<T> {
      var indexValue = index.Value;
      if (!index.IsFromEnd) {
         return sequence.At(indexValue);
      } else if (sequence.Count.Is(out var count)) {
         return sequence.At(count - indexValue);
      } else {
         var iterator = sequence.Iterator;
         if (Sequence<T>.Pop(ref iterator).Is(out var first)) {
            var queue = new Queue<T>();
            queue.Enqueue(first);

            iterator.Fold(new ValueTuple(), new AtFromEndFoldFunc<T>(indexValue, queue));

            if (queue.Count == indexValue) return queue.Dequeue();
         }

         return Option.None;
      }
   }
}