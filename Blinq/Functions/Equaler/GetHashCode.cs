using System.Collections.Generic;

namespace Blinq;

public static partial class Equaler {
   public static int GetHashCode<T, TEqualer> (T obj, Func<EqualerProvider<T>, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      var equaler = provideEqualer.Invoke();
      return equaler.GetHashCode(obj);
   }
}
