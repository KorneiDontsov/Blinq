namespace Blinq;

public static partial class Option {
   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, returns default value of <typeparamref name="T" />.
   /// </summary>
   /// <returns>
   ///    The value of the current <paramref name="option" /> if it exists; otherwise, default value of <typeparamref name="T" />.
   /// </returns>
   public static T? OrDefault<T> (this in Option<T> option) {
      return option.ValueOrDefault;
   }
}
