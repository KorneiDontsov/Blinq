namespace Blinq;

struct DropNumerationFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<NumeratedItem<T>, TAccumulator>
where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public DropNumerationFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (NumeratedItem<T> item, ref TAccumulator accumulator) {
      return InnerFoldFunc.Invoke(item.Value, ref accumulator);
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
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return Iterator.Fold(seed, new DropNumerationFoldFunc<T, TAccumulator, TFoldFunc>(func));
   }
}

public static partial class Sequence {
   /// <summary>Drops numeration of a sequence numerated with <see cref="Numerate" />.</summary>
   /// <returns>A sequence of the elements of the input sequence without their positions.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, DropNumerationIterator<T, TIterator>> DropNumeration<T, TIterator> (
      this in Sequence<NumeratedItem<T>, TIterator> sequence
   )
   where TIterator: IIterator<NumeratedItem<T>> {
      return new Sequence<T, DropNumerationIterator<T, TIterator>>(
         new DropNumerationIterator<T, TIterator>(sequence.Iterator),
         sequence.Count
      );
   }
}
