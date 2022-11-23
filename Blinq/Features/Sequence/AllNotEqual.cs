using Blinq.Functors;

namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllNotEqual<T, TIterator, TEqualer> (this in Sequence<T, TIterator> sequence, T value, TEqualer equaler)
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return sequence.All(new NotEqualPredicate<T, TEqualer>(value, equaler));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllNotEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return sequence.AllNotEqual(value, provideEqualer.Invoke());
   }


   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllNotEqual<T, TIterator> (this in Sequence<T, TIterator> sequence, T value) where TIterator: IIterator<T> {
      return sequence.AllNotEqual(value, Get<T>.Equaler.Default());
   }
}