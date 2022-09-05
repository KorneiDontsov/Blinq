namespace Blinq;

enum DefaultIfEmptyIteratorPosition {
   Start,
   Middle,
   End,
}

public struct DefaultIfEmptyIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly T DefaultValue;
   DefaultIfEmptyIteratorPosition Position;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DefaultIfEmptyIterator (TIterator iterator, T defaultValue) {
      Iterator = iterator;
      DefaultValue = defaultValue;
      Position = DefaultIfEmptyIteratorPosition.Start;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      switch (Position) {
         case DefaultIfEmptyIteratorPosition.Start when Sequence<T>.Pop(ref Iterator).Is(out var first): {
            Position = DefaultIfEmptyIteratorPosition.Middle;
            if (func.Invoke(first, ref seed)) {
               return seed;
            } else {
               goto case DefaultIfEmptyIteratorPosition.Middle;
            }
         }
         case DefaultIfEmptyIteratorPosition.Start: {
            func.Invoke(DefaultValue, ref seed);
            Position = DefaultIfEmptyIteratorPosition.End;
            return seed;
         }
         case DefaultIfEmptyIteratorPosition.Middle: {
            return Iterator.Fold(seed, func);
         }
         default: {
            return seed;
         }
      }
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, DefaultIfEmptyIterator<T, TIterator>> DefaultIfEmpty<T, TIterator> (
      this in Sequence<T, TIterator> sequence,
      T defaultValue = default!
   ) where TIterator: IIterator<T> {
      var count = sequence.Count switch {
         (true, 0) => Option.Value(1),
         _ => sequence.Count,
      };
      return Sequence<T>.Create(new DefaultIfEmptyIterator<T, TIterator>(sequence.Iterator, defaultValue), count);
   }
}
