using System.Runtime.CompilerServices;

namespace Blinq;

public static partial class Option {
   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, invokes <paramref name="func" /> and returns its result.
   /// </summary>
   /// <returns>
   ///    The value of <paramref name="option" /> if it exists; otherwise, the result of invocation of <paramref name="func" />.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T OrElse<T> (this in Option<T> option, Func<T> func) {
      return option.HasValue ? option.ValueOrDefault! : func();
   }
}
