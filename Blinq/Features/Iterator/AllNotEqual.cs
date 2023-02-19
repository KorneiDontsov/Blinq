using Blinq.Functors;

namespace Blinq;

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllNotEqual<T, TIterator, TEqualer> (this in Contract<IIterator<T>, TIterator> iterator, T value, TEqualer equaler)
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return iterator.All(new NotEqualPredicate<T, TEqualer>(value, equaler));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllNotEqual<T, TIterator, TEqualer> (
      this in Contract<IIterator<T>, TIterator> iterator,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return iterator.AllNotEqual(value, provideEqualer.Invoke());
   }


   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllNotEqual<T, TIterator> (this in Contract<IIterator<T>, TIterator> iterator, T value) where TIterator: IIterator<T> {
      return iterator.AllNotEqual(value, Get<T>.Equaler.Default());
   }
}
