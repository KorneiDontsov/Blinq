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

public struct NumerateIterator<T, TIterator>: IIterator<NumeratedItem<T>> where TIterator: IIterator<T> {
   TIterator Iterator;
   int Position;

   public NumerateIterator (TIterator iterator) {
      Iterator = iterator;
      Position = 0;
   }

   public NumeratedItem<T> Current => new(Iterator.Current, Position++);

   public bool MoveNext () {
      return Iterator.MoveNext();
   }
}

public struct DropNumerationIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<NumeratedItem<T>> {
   TIterator Iterator;

   public DropNumerationIterator (TIterator iterator) {
      Iterator = iterator;
   }

   public T Current => Iterator.Current.Value;

   public bool MoveNext () {
      return Iterator.MoveNext();
   }
}

public static partial class Sequence {
   /// <summary>Appends to each element its position in a sequence.</summary>
   /// <returns>A sequence of <see cref="NumeratedItem{T}" /> that contain the elements of the input sequence with their positions.</returns>
   public static Sequence<NumeratedItem<T>, NumerateIterator<T, TIterator>> Numerate<T, TIterator> (this in Sequence<T, TIterator> sequence)
   where TIterator: IIterator<T> {
      return new Sequence<NumeratedItem<T>, NumerateIterator<T, TIterator>>(
         new NumerateIterator<T, TIterator>(sequence.Iterator),
         sequence.Count
      );
   }

   /// <summary>Drops numeration of a sequence numerated with <see cref="Numerate" />.</summary>
   /// <returns>A sequence of the elements of the input sequence without their positions.</returns>
   public static Sequence<T, DropNumerationIterator<T, TIterator>> DropNumeration<T, TIterator> (this in Sequence<NumeratedItem<T>, TIterator> sequence)
   where TIterator: IIterator<NumeratedItem<T>> {
      return new Sequence<T, DropNumerationIterator<T, TIterator>>(
         new DropNumerationIterator<T, TIterator>(sequence.Iterator),
         sequence.Count
      );
   }
}
