using System.Runtime.CompilerServices;

namespace Blinq;

struct InspectAccumulator<T, TAccumulated, TNextAccumulator>: IAccumulator<T, TAccumulated> where TNextAccumulator: IAccumulator<T, TAccumulated> {
   readonly Action<T> Action;
   TNextAccumulator NextAccumulator;

   public InspectAccumulator (Action<T> action, TNextAccumulator nextAccumulator) {
      Action = action;
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulated accumulated) {
      Action(item);
      return NextAccumulator.Invoke(item, ref accumulated);
   }
}

public struct InspectIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly Action<T> Action;

   public InspectIterator (TIterator iterator, Action<T> action) {
      Iterator = iterator;
      Action = action;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated> {
      return Iterator.Accumulate(new InspectAccumulator<T, TAccumulated, TAccumulator>(Action, accumulator), seed);
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, InspectIterator<T, TIterator>> Inspect<T, TIterator> (this in Sequence<T, TIterator> sequence, Action<T> action)
   where TIterator: IIterator<T> {
      return new Sequence<T, InspectIterator<T, TIterator>>(
         new InspectIterator<T, TIterator>(sequence.Iterator, action),
         sequence.Count
      );
   }
}
