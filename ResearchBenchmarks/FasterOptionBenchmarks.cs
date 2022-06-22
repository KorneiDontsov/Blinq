using System.Runtime.CompilerServices;

namespace ResearchBenchmarks.FasterOption;

sealed class NoValueException: Exception {
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

readonly struct Option<T> {
   internal readonly T? ValueOrDefault;
   public readonly bool HasValue;

   public Option (T value) {
      ValueOrDefault = value;
      HasValue = true;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T InstanceValue () {
      if (!HasValue) NoValueException.Throw();
      return ValueOrDefault!;
   }

   public T InstanceOrElse (Func<T> func) {
      return HasValue ? ValueOrDefault! : func();
   }
}

static class OptionExtensions {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T ExtensionValue<T> (this Option<T> option) {
      if (!option.HasValue) NoValueException.Throw();
      return option.ValueOrDefault!;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T InExtensionValue<T> (this in Option<T> option) {
      if (!option.HasValue) NoValueException.Throw();
      return option.ValueOrDefault!;
   }

   public static T ExtensionOrElse<T> (this Option<T> option, Func<T> func) {
      return option.HasValue ? option.ValueOrDefault! : func();
   }

   public static T InExtensionOrElse<T> (this in Option<T> option, Func<T> func) {
      return option.HasValue ? option.ValueOrDefault! : func();
   }
}

public class FasterOptionBenchmarks {
   Option<object> Option;

   [Params(true, false)]
   public bool ValueOrNone;

   [GlobalSetup]
   public void Setup () {
      Option = ValueOrNone ? new Option<object>(new object()) : default;
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   Option<object> ReturnOption () {
      return Option;
   }

   [Benchmark]
   public object InstanceValue () {
      try {
         return ReturnOption().InstanceValue();
      } catch (NoValueException) {
         return new object();
      }
   }

   [Benchmark]
   public object ExtensionValue () {
      try {
         return ReturnOption().ExtensionValue();
      } catch (NoValueException) {
         return new object();
      }
   }

   [Benchmark]
   public object InExtensionValue () {
      try {
         return ReturnOption().InExtensionValue();
      } catch (NoValueException) {
         return new object();
      }
   }

   [Benchmark]
   public object InstanceOrElse () {
      return ReturnOption().InstanceOrElse(static () => new object());
   }

   [Benchmark]
   public object ExtensionOrElse () {
      return ReturnOption().ExtensionOrElse(static () => new object());
   }

   [Benchmark]
   public object InExtensionOrElse () {
      return ReturnOption().InExtensionOrElse(static () => new object());
   }
}
