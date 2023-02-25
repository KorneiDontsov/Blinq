using Blinq.Collections;

namespace Blinq;

public struct ExceptIterator<T, TEqualer, TIterator1, TIterator2>: IIterator<T>
where T: notnull
where TEqualer: IEqualityComparer<T>
where TIterator1: IIterator<T>
where TIterator2: IIterator<T> {
   TIterator1 Iterator1;
   TIterator2 Iterator2;
   ValueSet<T, TEqualer> Set;
   bool IsInitialized;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal ExceptIterator (TIterator1 iterator1, TIterator2 iterator2, TEqualer equaler) {
      Iterator1 = iterator1;
      Iterator2 = iterator2;
      Set = new(equaler);
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   void Initialize () {
      var collector = new ValueSetCollector<T, TEqualer>(Set);
      Set = Iterator.Collect<T, TIterator2, ValueSet<T, TEqualer>, ValueSetCollector<T, TEqualer>>(ref Iterator2, ref collector);
      IsInitialized = true;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      if (!IsInitialized) Initialize();
      (accumulator, Set) = Iterator1.Fold((seed: accumulator, Set), new DistinctFold<T, TAccumulator, TEqualer, TFold>(fold));
      return accumulator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      var result = Fold(Option<T>.None, new PopFold<T>());
      return result.Is(out item);
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
   public static Contract<IIterator<T>, ExceptIterator<T, TEqualer, TIterator1, TIterator2>> Except<T, TIterator1, TIterator2, TEqualer> (
      this in Contract<IIterator<T>, TIterator1> iterator1,
      in Contract<IIterator<T>, TIterator2> iterator2,
      TEqualer equaler
   )
   where T: notnull
   where TIterator1: IIterator<T>
   where TIterator2: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return new ExceptIterator<T, TEqualer, TIterator1, TIterator2>(iterator1, iterator2, equaler);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, ExceptIterator<T, TEqualer, TIterator1, TIterator2>> Except<T, TIterator1, TIterator2, TEqualer> (
      this in Contract<IIterator<T>, TIterator1> iterator1,
      in Contract<IIterator<T>, TIterator2> iterator2,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where T: notnull
   where TIterator1: IIterator<T>
   where TIterator2: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return iterator1.Except(iterator2, provideEqualer());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, ExceptIterator<T, DefaultEqualer<T>, TIterator1, TIterator2>> Except<T, TIterator1, TIterator2> (
      this in Contract<IIterator<T>, TIterator1> iterator1,
      in Contract<IIterator<T>, TIterator2> iterator2
   )
   where T: notnull
   where TIterator1: IIterator<T>
   where TIterator2: IIterator<T> {
      return iterator1.Except(iterator2, Get<T>.Equaler.Default());
   }
}
