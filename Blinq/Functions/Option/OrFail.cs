namespace Blinq;

public static partial class Option {
   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, throws <see cref="NoValueException" /> with the
   ///    specified <paramref name="message" />.
   /// </summary>
   /// <returns>The value of the current <see cref="Option{T}" />.</returns>
   /// <exception cref="NoValueException">The current <see cref="Option{T}" /> has not value. </exception>
   public static T OrFail<T> (this in Option<T> option, string message) {
      if (!option.HasValue) NoValueException.Throw(message);
      return option.ValueOrDefault!;
   }
}
