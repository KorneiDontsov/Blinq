using Blinq.Functors;

namespace Blinq;

readonly struct AllFoldFunc<T, TPredicate>: IFoldFunc<T, bool> where TPredicate: IPredicate<T> {
   readonly TPredicate Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AllFoldFunc (TPredicate predicate) {
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

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool All<T, TIterator, TPredicate> (this in Sequence<T, TIterator> sequence, TPredicate predicate)
   where TIterator: IIterator<T>
   where TPredicate: IPredicate<T> {
      return sequence.Iterator.Fold(true, new AllFoldFunc<T, TPredicate>(predicate));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool All<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, bool> predicate) where TIterator: IIterator<T> {
      return sequence.All(new FuncPredicate<T>(predicate));
   }
}
