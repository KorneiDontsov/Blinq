using System.Runtime.CompilerServices;

namespace Blinq;

public readonly struct NumeratedItem<T> {
   public readonly T Value;
   public readonly int Position;

   public NumeratedItem (T value, int position) {
      Value = value;
      Position = position;
   }

   public void Deconstruct (out T value, out int position) {
      value = Value;
      position = Position;
   }
}

struct NumerateAccumulator<T, TAccumulated, TNextAccumulator>: IAccumulator<T, (TAccumulated Accumulated, int Position)>
where TNextAccumulator: IAccumulator<NumeratedItem<T>, TAccumulated> {
   TNextAccumulator NextAccumulator;

   public NumerateAccumulator (TNextAccumulator nextAccumulator) {
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulated Accumulated, int Position) state) {
      return NextAccumulator.Invoke(new NumeratedItem<T>(item, state.Position++), ref state.Accumulated);
   }
}

public struct NumerateIterator<T, TIterator>: IIterator<NumeratedItem<T>> where TIterator: IIterator<T> {
   TIterator Iterator;
   int Position;

   public NumerateIterator (TIterator iterator) {
      Iterator = iterator;
      Position = 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<NumeratedItem<T>, TAccumulated> {
      (seed, Position) = Iterator.Accumulate(new NumerateAccumulator<T, TAccumulated, TAccumulator>(accumulator), (seed, Position));
      return seed;
   }
}

struct DropNumerationAccumulator<T, TAccumulated, TNextAccumulator>: IAccumulator<NumeratedItem<T>, TAccumulated>
where TNextAccumulator: IAccumulator<T, TAccumulated> {
   TNextAccumulator NextAccumulator;

   public DropNumerationAccumulator (TNextAccumulator nextAccumulator) {
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (NumeratedItem<T> item, ref TAccumulated accumulated) {
      return NextAccumulator.Invoke(item.Value, ref accumulated);
   }
}

public struct DropNumerationIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<NumeratedItem<T>> {
   TIterator Iterator;

   public DropNumerationIterator (TIterator iterator) {
      Iterator = iterator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated> {
      return Iterator.Accumulate(new DropNumerationAccumulator<T, TAccumulated, TAccumulator>(accumulator), seed);
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
