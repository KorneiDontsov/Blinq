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

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      switch (Position) {
         case DefaultIfEmptyIteratorPosition.Start when Iterator.TryPop(out item): {
            Position = DefaultIfEmptyIteratorPosition.Middle;
            return true;
         }
         case DefaultIfEmptyIteratorPosition.Start: {
            Position = DefaultIfEmptyIteratorPosition.End;
            item = DefaultValue;
            return true;
         }
         case DefaultIfEmptyIteratorPosition.Middle: {
            return Iterator.TryPop(out item);
         }
         default: {
            item = default;
            return false;
         }
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      switch (Position) {
         case DefaultIfEmptyIteratorPosition.Start when Iterator.TryPop(out var first): {
            Position = DefaultIfEmptyIteratorPosition.Middle;
            if (fold.Invoke(first, ref accumulator)) {
               return accumulator;
            } else {
               goto case DefaultIfEmptyIteratorPosition.Middle;
            }
         }
         case DefaultIfEmptyIteratorPosition.Start: {
            Position = DefaultIfEmptyIteratorPosition.End;
            _ = fold.Invoke(DefaultValue, ref accumulator);
            return accumulator;
         }
         case DefaultIfEmptyIteratorPosition.Middle: {
            return Iterator.Fold(accumulator, fold);
         }
         default: {
            return accumulator;
         }
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      switch (Position) {
         case DefaultIfEmptyIteratorPosition.Start: {
            if (Iterator.TryGetCount(out count)) {
               if (count == 0) count = 1;
               return true;
            } else {
               return false;
            }
         }
         case DefaultIfEmptyIteratorPosition.Middle: {
            return Iterator.TryGetCount(out count);
         }
         default: {
            count = 0;
            return false;
         }
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, DefaultIfEmptyIterator<T, TIterator>> DefaultIfEmpty<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T defaultValue = default!
   ) where TIterator: IIterator<T> {
      return new DefaultIfEmptyIterator<T, TIterator>(iterator, defaultValue);
   }
}
