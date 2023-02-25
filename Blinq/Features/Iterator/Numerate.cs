namespace Blinq;

readonly struct NumerateFold<T, TAccumulator, TInnerFold>: IFold<T, (TAccumulator Accumulator, int Position)>
where TInnerFold: IFold<NumeratedItem<T>, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NumerateFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator Accumulator, int Position) state) {
      return InnerFold.Invoke(new NumeratedItem<T>(item, state.Position++), ref state.Accumulator);
   }
}

public struct NumerateIterator<T, TIterator>: IIterator<NumeratedItem<T>> where TIterator: IIterator<T> {
   TIterator Iterator;
   int Position;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NumerateIterator (TIterator iterator) {
      Iterator = iterator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop (out NumeratedItem<T> item) {
      if (Iterator.TryPop(out var underlyingItem)) {
         item = new NumeratedItem<T>(underlyingItem, Position++);
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold)
   where TFold: IFold<NumeratedItem<T>, TAccumulator> {
      (accumulator, Position) = Iterator.Fold((seed: accumulator, Position), new NumerateFold<T, TAccumulator, TFold>(fold));
      return accumulator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Iterator.TryGetCount(out count);
   }
}

public static partial class Iterator {
   /// <summary>Appends to each element its position in a sequence.</summary>
   /// <returns>A sequence of <see cref="NumeratedItem{T}" /> that contain the elements of the input sequence with their positions.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<NumeratedItem<T>>, NumerateIterator<T, TIterator>> Numerate<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator
   )
   where TIterator: IIterator<T> {
      return new NumerateIterator<T, TIterator>(iterator);
   }
}
