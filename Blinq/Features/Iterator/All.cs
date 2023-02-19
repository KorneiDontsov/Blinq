using Blinq.Functors;

namespace Blinq;

readonly struct AllFold<T, TPredicate>: IFold<T, bool> where TPredicate: IPredicate<T> {
   readonly TPredicate Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AllFold (TPredicate predicate) {
      Predicate = predicate;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref bool accumulator) {
      if (Predicate.Invoke(item)) {
         return false;
      } else {
         accumulator = false;
         return true;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool All<T, TIterator, TPredicate> (this in Contract<IIterator<T>, TIterator> iterator, TPredicate predicate)
   where TIterator: IIterator<T>
   where TPredicate: IPredicate<T> {
      return iterator.Value.Fold(true, new AllFold<T, TPredicate>(predicate));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool All<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, Func<T, bool> predicate) where TIterator: IIterator<T> {
      return iterator.All(new FuncPredicate<T>(predicate));
   }
}
