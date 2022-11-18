namespace Blinq;

public struct PrependIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly T Element;
   bool Prepended;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public PrependIterator (TIterator iterator, T element) {
      Iterator = iterator;
      Element = element;
      Prepended = false;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      if (!Prepended) {
         Prepended = true;
         if (func.Invoke(Element, ref seed)) return seed;
      }

      return Iterator.Fold(seed, func);
   }
}

public static partial class Sequence {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, PrependIterator<T, TIterator>> Prepend<T, TIterator> (this in Sequence<T, TIterator> sequence, T element)
   where TIterator: IIterator<T> {
      var newCount = sequence.Count switch {
         (true, var count) => Option.Value(checked(count + 1)),
         _ => Option.None,
      };
      return Sequence<T>.Create(new PrependIterator<T, TIterator>(sequence.Iterator, element), newCount);
   }
}
