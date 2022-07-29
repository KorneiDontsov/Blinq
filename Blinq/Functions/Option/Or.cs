using System.Runtime.CompilerServices;

namespace Blinq;

public static partial class Option {
   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, returns <paramref name="elseValue" />.
   /// </summary>
   /// <param name="elseValue">The value to return if <paramref name="option" /> has no value.</param>
   /// <returns>
   ///    The value of <paramref name="option" /> if it exists; otherwise, <paramref name="elseValue" />.
   /// </returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Or<T> (this in Option<T> option, T elseValue) {
      return option.HasValue ? option.ValueOrDefault! : elseValue;
   }
}
