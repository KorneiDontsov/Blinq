using System.Runtime.CompilerServices;

namespace Blinq;

struct WhereAccumulator<T, TAccumulated, TNextAccumulator>: IAccumulator<T, TAccumulated> where TNextAccumulator: IAccumulator<T, TAccumulated> {
   readonly Func<T, bool> Predicate;
   TNextAccumulator NextAccumulator;

   public WhereAccumulator (Func<T, bool> predicate, TNextAccumulator nextAccumulator) {
      Predicate = predicate;
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulated accumulated) {
      return Predicate(item) && NextAccumulator.Invoke(item, ref accumulated);
   }
}

public struct WhereIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly Func<T, bool> Predicate;

   public WhereIterator (TIterator iterator, Func<T, bool> predicate) {
      Iterator = iterator;
      Predicate = predicate;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated> {
      return Iterator.Accumulate(new WhereAccumulator<T, TAccumulated, TAccumulator>(Predicate, accumulator), seed);
   }
}

public static partial class Sequence {
   /// <summary>Filters a sequence of values based on a predicate.</summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <returns>A sequence that contains elements from the input sequence that satisfy the condition.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, TIterator>> Where<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, bool> predicate)
   where TIterator: IIterator<T> {
      return new WhereIterator<T, TIterator>(sequence.Iterator, predicate);
   }
}
