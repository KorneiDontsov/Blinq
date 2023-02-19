using Blinq.Functors;

namespace Blinq;

readonly struct WhereFold<T, TAccumulator, TPredicate, TInnerFold>: IFold<T, TAccumulator>
where TPredicate: IPredicate<T>
where TInnerFold: IFold<T, TAccumulator> {
   readonly TPredicate Predicate;
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereFold (TPredicate predicate, TInnerFold innerFold) {
      Predicate = predicate;
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return Predicate.Invoke(item) && InnerFold.Invoke(item, ref accumulator);
   }
}

readonly struct WherePopFold<T, TPredicate>: IFold<T, Option<T>> where TPredicate: IPredicate<T> {
   readonly TPredicate Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WherePopFold (TPredicate predicate) {
      Predicate = predicate;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref Option<T> accumulator) {
      if (Predicate.Invoke(item)) {
         accumulator = item;
         return true;
      } else {
         return false;
      }
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
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      var result = Iterator.Fold(Option<T>.None, new WherePopFold<T, TPredicate>(Predicate));
      return result.Is(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      return Iterator.Fold(seed, new WhereFold<T, TAccumulator, TPredicate, TFold>(Predicate, fold));
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = default;
      return false;
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, TPredicate, TIterator>> Where<T, TIterator, TPredicate> (
      this in Contract<IIterator<T>, TIterator> iterator,
      TPredicate predicate
   )
   where TIterator: IIterator<T>
   where TPredicate: IPredicate<T> {
      return new WhereIterator<T, TPredicate, TIterator>(iterator, predicate);
   }

   /// <summary>Filters a sequence of values based on a predicate.</summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <returns>A sequence that contains elements from the input <paramref name="iterator" /> that satisfy the condition.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, WhereIterator<T, FuncPredicate<T>, TIterator>> Where<T, TIterator> (
      this in Contract<IIterator<T>, TIterator> iterator,
      Func<T, bool> predicate
   )
   where TIterator: IIterator<T> {
      return iterator.Where(new FuncPredicate<T>(predicate));
   }
}
