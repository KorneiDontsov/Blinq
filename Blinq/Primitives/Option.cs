namespace Blinq;

public sealed class NoValueException: Exception {
   public NoValueException () { }

   public NoValueException (string? message): base(message) { }

   [DoesNotReturn] [MethodImpl(MethodImplOptions.NoInlining)]
   internal static void Throw (string message) {
      throw new NoValueException(message);
   }
}

/// <summary>Represents an optional value.</summary>
/// <typeparam name="T">The type of a value.</typeparam>
public readonly struct Option<T> {
   internal readonly T? ValueOrDefault;
   public readonly bool HasValue;

   /// <summary>Creates an <see cref="Option{T}" /> with the specified <paramref name="value" />.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Option (T value) {
      ValueOrDefault = value;
      HasValue = true;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Deconstruct (out bool hasValue, out T valueOrDefault) {
      hasValue = HasValue;
      valueOrDefault = ValueOrDefault!;
   }

   /// <summary>An <see cref="Option{T}" /> without value.</summary>
   [Pure] public static Option<T> None => default;

   /// <summary>Creates an <see cref="Option{T}" /> with the specified <paramref name="value" />.</summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Option<T> (T value) {
      return new Option<T>(value);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Option<T> (ValueTuple _) {
      return default;
   }
}

public static class Option {
   [Pure] public static ValueTuple None => default;

   /// <summary>Creates an <see cref="Option{T}" /> with the specified <paramref name="value" />.</summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Option<T> Value<T> (T value) {
      return new Option<T>(value);
   }

   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, throws <see cref="NoValueException" />.
   /// </summary>
   /// <returns>The underlying value of <paramref name="option" />.</returns>
   /// <exception cref="NoValueException">The current <see cref="Option{T}" /> has not value. </exception>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Value<T> (this in Option<T> option) {
      if (!option.HasValue) Get.Throw<NoValueException>();
      return option.ValueOrDefault!;
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Is<T> (this in Option<T> option, [MaybeNullWhen(false)] out T value) {
      value = option.ValueOrDefault!;
      return option.HasValue;
   }

   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, returns <paramref name="elseValue" />.
   /// </summary>
   /// <param name="elseValue">The value to return if <paramref name="option" /> has no value.</param>
   /// <returns>
   ///    The value of <paramref name="option" /> if it exists; otherwise, <paramref name="elseValue" />.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Or<T> (this in Option<T> option, T elseValue) {
      return option.HasValue ? option.ValueOrDefault! : elseValue;
   }

   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, returns default value of <typeparamref name="T" />.
   /// </summary>
   /// <returns>
   ///    The value of the current <paramref name="option" /> if it exists; otherwise, default value of <typeparamref name="T" />.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T? OrDefault<T> (this in Option<T> option) {
      return option.ValueOrDefault;
   }

   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, invokes <paramref name="func" /> and returns its result.
   /// </summary>
   /// <returns>
   ///    The value of <paramref name="option" /> if it exists; otherwise, the result of invocation of <paramref name="func" />.
   /// </returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T OrElse<T> (this in Option<T> option, Func<T> func) {
      return option.HasValue ? option.ValueOrDefault! : func();
   }

   /// <summary>
   ///    If <paramref name="option" /> has a value then returns it; otherwise, throws <see cref="NoValueException" /> with the
   ///    specified <paramref name="message" />.
   /// </summary>
   /// <returns>The value of the current <see cref="Option{T}" />.</returns>
   /// <exception cref="NoValueException">The current <see cref="Option{T}" /> has not value. </exception>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T OrFail<T> (this in Option<T> option, string message) {
      if (!option.HasValue) NoValueException.Throw(message);
      return option.ValueOrDefault!;
   }
}
