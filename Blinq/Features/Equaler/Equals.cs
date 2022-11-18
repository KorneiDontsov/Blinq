namespace Blinq;

public static partial class Equaler {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Equals<T, TEqualer> (this T a, T b, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      var equaler = provideEqualer.Invoke();
      return equaler.Equals(a, b);
   }
}
