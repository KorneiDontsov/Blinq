namespace Blinq;

public struct AppendIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly T Element;
   bool Appended;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AppendIterator (TIterator iterator, T element) {
      Iterator = iterator;
      Element = element;
      Appended = false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      if (!Appended) {
         (seed, var interrupted) = Iterator.Fold((Accumulator: seed, Interrupted: false), new InterruptingFoldFunc<T, TAccumulator, TFoldFunc>(func));
         if (!interrupted) {
            _ = func.Invoke(Element, ref seed);
            Appended = true;
         }
      }

      return seed;
   }
}

public static partial class Sequence {
   public static Sequence<T, AppendIterator<T, TIterator>> Append<T, TIterator> (this in Sequence<T, TIterator> sequence, T element)
   where TIterator: IIterator<T> {
      return new Sequence<T, AppendIterator<T, TIterator>>(
         new AppendIterator<T, TIterator>(sequence.Iterator, element),
         sequence.Count switch {
            (true, var count) => Option.Value(checked(count + 1)),
            _ => Option.None,
         }
      );
   }
}
