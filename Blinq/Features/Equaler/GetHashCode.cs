namespace Blinq;

public static partial class Equaler {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int GetHashCode<T, TEqualer> (this T obj, ProvideEqualer<T, TEqualer> provideEqualer)
   where T: notnull
   where TEqualer: IEqualityComparer<T> {
      var equaler = provideEqualer.Invoke();
      return equaler.GetHashCode(obj);
   }
}
