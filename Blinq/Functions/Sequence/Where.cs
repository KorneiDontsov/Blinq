namespace Blinq;

readonly struct WhereFoldFunc<T, TAccumulator, TPredicate, TInnerFoldFunc>: IFoldFunc<T, TAccumulator>
where TPredicate: IPredicate<T>
where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   readonly TPredicate Predicate;
   readonly TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereFoldFunc (TPredicate predicate, TInnerFoldFunc innerFoldFunc) {
      Predicate = predicate;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return Predicate.Invoke(item) && InnerFoldFunc.Invoke(item, ref accumulator);
   }
}

public struct WhereIterator<T, TPredicate, TIterator>: IIterator<T>
where TPredicate: IPredicate<T>
where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly TPredicate Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereIterator (TIterator iterator, TPredicate predicate) {
      Iterator = iterator;
      Predicate = predicate;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return Iterator.Fold(seed, new WhereFoldFunc<T, TAccumulator, TPredicate, TFoldFunc>(Predicate, func));
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, TPredicate, TIterator>> Where<T, TIterator, TPredicate> (
      this in Sequence<T, TIterator> sequence,
      TPredicate predicate
   )
   where TIterator: IIterator<T>
   where TPredicate: IPredicate<T> {
      return new WhereIterator<T, TPredicate, TIterator>(sequence.Iterator, predicate);
   }

   /// <summary>Filters a sequence of values based on a predicate.</summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <returns>A sequence that contains elements from the input <paramref name="sequence" /> that satisfy the condition.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, FuncPredicate<T>, TIterator>> Where<T, TIterator> (
      this in Sequence<T, TIterator> sequence,
      Func<T, bool> predicate
   )
   where TIterator: IIterator<T> {
      return sequence.Where(new FuncPredicate<T>(predicate));
   }
}
