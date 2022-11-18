namespace Blinq;

readonly struct NumerateFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, (TAccumulator Accumulator, int Position)>
where TInnerFoldFunc: IFoldFunc<NumeratedItem<T>, TAccumulator> {
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NumerateFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator Accumulator, int Position) state) {
      return InnerFoldFunc.Invoke(new NumeratedItem<T>(item, state.Position++), ref state.Accumulator);
   }
}

public struct NumerateIterator<T, TIterator>: IIterator<NumeratedItem<T>> where TIterator: IIterator<T> {
   TIterator Iterator;
   int Position;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NumerateIterator (TIterator iterator) {
      Iterator = iterator;
      Position = 0;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func)
   where TFoldFunc: IFoldFunc<NumeratedItem<T>, TAccumulator> {
      (seed, Position) = Iterator.Fold((seed, Position), new NumerateFoldFunc<T, TAccumulator, TFoldFunc>(func));
      return seed;
   }
}

public static partial class Sequence {
   /// <summary>Appends to each element its position in a sequence.</summary>
   /// <returns>A sequence of <see cref="NumeratedItem{T}" /> that contain the elements of the input sequence with their positions.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<NumeratedItem<T>, NumerateIterator<T, TIterator>> Numerate<T, TIterator> (this in Sequence<T, TIterator> sequence)
   where TIterator: IIterator<T> {
      return Sequence<NumeratedItem<T>>.Create(new NumerateIterator<T, TIterator>(sequence.Iterator), sequence.Count);
   }
}
