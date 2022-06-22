namespace Blinq;

public struct FlattenIterator<TOut, TOutIterator, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<Sequence<TOut, TOutIterator>>
where TOutIterator: IIterator<TOut> {
   TInIterator InIterator;
   TOutIterator OutIterator;
   bool Started;

   public FlattenIterator (TInIterator inIterator) {
      InIterator = inIterator;
      OutIterator = default!;
      Started = false;
   }

   public TOut Current => OutIterator.Current;

   public bool MoveNext () {
      if (!Started) {
         Started = true;
      } else if (OutIterator.MoveNext()) {
         return true;
      }

      while (InIterator.MoveNext()) {
         OutIterator = InIterator.Current.Iterator;
         if (OutIterator.MoveNext()) return true;
      }

      return false;
   }
}

public static partial class Sequence {
   /// <summary>Flattens a sequence of the sequences into one sequence.</summary>
   /// <returns>A sequence whose elements are the elements of the input sequences.</returns>
   public static Sequence<T, FlattenIterator<T, TInnerIterator, TIterator>> Flatten<T, TIterator, TInnerIterator> (
      this in Sequence<Sequence<T, TInnerIterator>, TIterator> sequence
   )
   where TIterator: IIterator<Sequence<T, TInnerIterator>>
   where TInnerIterator: IIterator<T> {
      return new FlattenIterator<T, TInnerIterator, TIterator>(sequence.Iterator);
   }
}
