namespace Blinq;

readonly struct InspectFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, TAccumulator> where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   readonly Action<T> Action;
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public InspectFoldFunc (Action<T> action, TInnerFoldFunc innerFoldFunc) {
      Action = action;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      Action(item);
      return InnerFoldFunc.Invoke(item, ref accumulator);
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
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return Iterator.Fold(seed, new InspectFoldFunc<T, TAccumulator, TFoldFunc>(Action, func));
   }
}

public static partial class Sequence {
   /// <summary>Does something with each element of an iterator, passing the value on.</summary>
   /// <param name="action">An action to be executed on each element of <paramref name="sequence" />.</param>
   /// <returns>
   ///    A sequence that is equal to input <paramref name="sequence" /> but with <paramref name="action" /> to execute on each element.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, InspectIterator<T, TIterator>> Inspect<T, TIterator> (this in Sequence<T, TIterator> sequence, Action<T> action)
   where TIterator: IIterator<T> {
      return Sequence<T>.Create(new InspectIterator<T, TIterator>(sequence.Iterator, action), sequence.Count);
   }
}
