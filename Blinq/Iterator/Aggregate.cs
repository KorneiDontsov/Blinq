using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct AggregateVisitor<T, TAccumulate, TFunctor>
   : IIteratorVisitor<T, TAccumulate>
where TFunctor: IFunctor<TAccumulate, T, TAccumulate> {
   public required TFunctor func { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Visit (ref TAccumulate state, in T item) {
      state = this.func.Invoke(in state, in item);
      return false;
   }
}

public static partial class Iterator {
   public static TAccumulate Aggregate<T, TIterator, TAccumulate, TFunctor> (
      this Pin<IIterator<T>, TIterator> iterator,
      TAccumulate seed,
      Pin<IFunctor<TAccumulate, T, TAccumulate>, TFunctor> functor
   )
   where TIterator: IIterator<T>
   where TFunctor: IFunctor<TAccumulate, T, TAccumulate> {
      iterator.value.Accept(
         ref seed,
         new AggregateVisitor<T, TAccumulate, TFunctor> { func = functor }
      );
      return seed;
   }

   public static Option<T> Aggregate<T, TIterator, TFunctor> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<IFunctor<T, T, T>, TFunctor> functor
   )
   where TIterator: IIterator<T>
   where TFunctor: IFunctor<T, T, T> {
      if (!iterator.value.TryPop(out var first)) return Option.none;

      iterator.value.Accept(
         ref first,
         new AggregateVisitor<T, T, TFunctor> { func = functor }
      );
      return first;
   }
}
