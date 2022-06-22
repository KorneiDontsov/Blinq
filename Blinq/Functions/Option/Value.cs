using System.Runtime.CompilerServices;

namespace Blinq;

public static partial class Option {
   /// <summary>Creates an <see cref="Option{T}" /> with the specified <paramref name="value" />.</summary>
   public static Option<T> Value<T> (T value) {
      return new Option<T>(value);
   }

   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, throws <see cref="NoValueException" />.
   /// </summary>
   /// <returns>The underlying value of <paramref name="option" />.</returns>
   /// <exception cref="NoValueException">The current <see cref="Option{T}" /> has not value. </exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Value<T> (this in Option<T> option) {
      if (!option.HasValue) NoValueException.Throw();
      return option.ValueOrDefault!;
   }
}
