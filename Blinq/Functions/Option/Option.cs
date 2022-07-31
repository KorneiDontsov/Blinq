namespace Blinq;

public sealed class NoValueException: Exception {
   public NoValueException () { }

   public NoValueException (string? message): base(message) { }

   [MethodImpl(MethodImplOptions.NoInlining)]
   internal static void Throw () {
      throw new NoValueException();
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
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

   /// <summary>An <see cref="Option{T}" /> without value.</summary>
   public static Option<T> None { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default; }

   /// <summary>Creates an <see cref="Option{T}" /> with the specified <paramref name="value" />.</summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Option<T> (T value) {
      return new Option<T>(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Option<T> (ValueTuple _) {
      return default;
   }
}

public static partial class Option {
   public static ValueTuple None { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default; }
}
