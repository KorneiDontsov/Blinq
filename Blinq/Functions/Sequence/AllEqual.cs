using System.Collections.Generic;

namespace Blinq;

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllEqual<T, TIterator, TEqualer> (this in Sequence<T, TIterator> sequence, T value, TEqualer equaler)
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return sequence.All(new EqualPredicate<T, TEqualer>(value, equaler));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllEqual<T, TIterator, TEqualer> (
      this in Sequence<T, TIterator> sequence,
      T value,
      ProvideEqualer<T, TEqualer> provideEqualer
   )
   where TIterator: IIterator<T>
   where TEqualer: IEqualityComparer<T> {
      return sequence.AllEqual(value, provideEqualer.Invoke());
   }


   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool AllEqual<T, TIterator> (this in Sequence<T, TIterator> sequence, T value) where TIterator: IIterator<T> {
      return sequence.AllEqual(value, Equaler.Default<T>());
   }
}
