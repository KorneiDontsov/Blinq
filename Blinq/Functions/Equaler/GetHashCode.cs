namespace Blinq;

public static partial class Equaler {
   public static int GetHashCode<T, TEqualer> (this T obj, ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      var equaler = provideEqualer.Invoke();
      return equaler.GetHashCode(obj);
   }
}
