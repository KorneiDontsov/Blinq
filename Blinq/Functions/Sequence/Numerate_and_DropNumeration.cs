namespace Blinq;

public readonly struct NumeratedItem<T> {
   public readonly T Value;
   public readonly int Position;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public NumeratedItem (T value, int position) {
      Value = value;
      Position = position;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Deconstruct (out T value, out int position) {
      value = Value;
      position = Position;
   }
}

struct NumerateFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, (TAccumulator Accumulator, int Position)>
where TInnerFoldFunc: IFoldFunc<NumeratedItem<T>, TAccumulator> {
   TInnerFoldFunc InnerFoldFunc;

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
   /// <summary>Appends to each element its position in a sequence.</summary>
   /// <returns>A sequence of <see cref="NumeratedItem{T}" /> that contain the elements of the input sequence with their positions.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<NumeratedItem<T>, NumerateIterator<T, TIterator>> Numerate<T, TIterator> (this in Sequence<T, TIterator> sequence)
   where TIterator: IIterator<T> {
      return new Sequence<NumeratedItem<T>, NumerateIterator<T, TIterator>>(
         new NumerateIterator<T, TIterator>(sequence.Iterator),
         sequence.Count
      );
   }

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
