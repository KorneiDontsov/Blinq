namespace Blinq;

public struct AppendIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly T Element;
   bool Appended;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AppendIterator (TIterator iterator, T element) {
      Iterator = iterator;
      Element = element;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Appended) {
         item = default;
         return false;
      } else if (Iterator.TryPop(out item)) {
         return true;
      } else {
         Appended = true;
         item = Element;
         return true;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      if (!Appended) {
         (seed, var interrupted) = Iterator.Fold((Accumulator: seed, Interrupted: false), new InterruptingFold<T, TAccumulator, TFold>(fold));
         if (!interrupted) {
            _ = fold.Invoke(Element, ref seed);
            Appended = true;
         }
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (Appended) {
         count = 0;
         return true;
      } else if (Iterator.TryGetCount(out count) && count < int.MaxValue) {
         ++count;
         return true;
      } else {
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, AppendIterator<T, TIterator>> Append<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T element
   ) where TIterator: IIterator<T> {
      return new AppendIterator<T, TIterator>(iterator, element);
   }
}
