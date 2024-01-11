using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct AnyVisitor<T, TPredicate>: IIteratorVisitor<T, bool>
where TPredicate: IFunctor<T, bool> {
   public required TPredicate predicate { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Visit (ref bool state, in T item) {
      var hasAny = this.predicate.Invoke(in item);
      state = hasAny;
      return hasAny;
   }
}

public static partial class Iterator {
   public static bool Any<T, TIterator, TPredicate> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<IFunctor<T, bool>, TPredicate> predicate
   )
   where TIterator: IIterator<T>
   where TPredicate: IFunctor<T, bool> {
      var result = false;
      iterator.value.Accept(
         ref result,
         new AnyVisitor<T, TPredicate> { predicate = predicate }
      );
      return result;
   }

   public static bool All<T, TIterator, TPredicate> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<IFunctor<T, bool>, TPredicate> predicate
   )
   where TIterator: IIterator<T>
   where TPredicate: IFunctor<T, bool> {
      return !Any(iterator, predicate);
   }
}
