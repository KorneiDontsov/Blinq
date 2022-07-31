using System.Collections.Generic;

namespace Blinq;

public static partial class Equaler {
   public static bool Equals<T, TEqualer> (T a, T b, Func<EqualerProvider<T>, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      var equaler = provideEqualer.Invoke();
      return equaler.Equals(a, b);
   }
}
