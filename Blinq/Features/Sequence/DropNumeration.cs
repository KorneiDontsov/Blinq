namespace Blinq;

readonly struct DropNumerationFold<T, TAccumulator, TInnerFold>: IFold<NumeratedItem<T>, TAccumulator>
where TInnerFold: IFold<T, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DropNumerationFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (NumeratedItem<T> item, ref TAccumulator accumulator) {
      return InnerFold.Invoke(item.Value, ref accumulator);
   }
}

public struct DropNumerationIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<NumeratedItem<T>> {
   TIterator Iterator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DropNumerationIterator (TIterator iterator) {
      Iterator = iterator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Iterator.TryPop(out var underlyingItem)) {
         item = underlyingItem.Value;
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      return Iterator.Fold(seed, new DropNumerationFold<T, TAccumulator, TFold>(fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Iterator.TryGetCount(out count);
   }
}

public static partial class Iterator {
   /// <summary>Drops numeration of a sequence numerated with <see cref="Numerate" />.</summary>
   /// <returns>A sequence of the elements of the input sequence without their positions.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, DropNumerationIterator<T, TIterator>> DropNumeration<T, TIterator> (
      this in Contract<IIterator<NumeratedItem<T>>, TIterator> iterator
   )
   where TIterator: IIterator<NumeratedItem<T>> {
      return new DropNumerationIterator<T, TIterator>(iterator);
   }
}
