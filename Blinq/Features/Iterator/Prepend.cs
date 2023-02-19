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
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (!Prepended) {
         Prepended = true;
         item = Element;
         return true;
      }

      return Iterator.TryPop(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      if (!Prepended) {
         Prepended = true;
         if (fold.Invoke(Element, ref seed)) return seed;
      }

      return Iterator.Fold(seed, fold);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (!Iterator.TryGetCount(out count)) {
         return false;
      } else if (!Prepended) {
         return true;
      } else if (count < int.MaxValue) {
         ++count;
         return true;
      } else {
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, PrependIterator<T, TIterator>> Prepend<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T element
   )
   where TIterator: IIterator<T> {
      return new PrependIterator<T, TIterator>(iterator, element);
   }
}
