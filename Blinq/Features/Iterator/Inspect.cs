namespace Blinq;

readonly struct InspectFold<T, TAccumulator, TInnerFold>: IFold<T, TAccumulator> where TInnerFold: IFold<T, TAccumulator> {
   readonly Action<T> Action;
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public InspectFold (Action<T> action, TInnerFold innerFold) {
      Action = action;
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      Action(item);
      return InnerFold.Invoke(item, ref accumulator);
   }
}

public struct InspectIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly Action<T> Action;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public InspectIterator (TIterator iterator, Action<T> action) {
      Iterator = iterator;
      Action = action;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Iterator.TryPop(out item)) {
         Action(item);
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      return Iterator.Fold(accumulator, new InspectFold<T, TAccumulator, TFold>(Action, fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Iterator.TryGetCount(out count);
   }
}

public static partial class Iterator {
   /// <summary>Does something with each element of an iterator, passing the value on.</summary>
   /// <param name="action">An action to be executed on each element of <paramref name="iterator" />.</param>
   /// <returns>
   ///    A sequence that is equal to input <paramref name="iterator" /> but with <paramref name="action" /> to execute on each element.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, InspectIterator<T, TIterator>> Inspect<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Action<T> action
   )
   where TIterator: IIterator<T> {
      return new InspectIterator<T, TIterator>(iterator, action);
   }
}
