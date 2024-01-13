using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

public readonly struct NoneOption { }

/// <summary>Represents an optional value.</summary>
/// <typeparam name="T">The type of a value.</typeparam>
public readonly partial struct Option<T> {
   internal readonly T? _value;
   internal readonly bool _hasValue;

   public bool hasValue => this._hasValue;

   public T value {
      internal get {
         Assert.Debug(this.hasValue);
         return this._value!;
      }
      init {
         this._value = value;
         this._hasValue = true;
      }
   }

   /// <returns>An <see cref="Option{T}" /> without value.</returns>
   public static Option<T> none => default;

   /// <returns>
   ///    An <see cref="Option{T}" /> with the specified <paramref name="value" />.
   /// </returns>
   public static implicit operator Option<T> (T value) {
      return new Option<T> { value = value };
   }

   /// <returns>An <see cref="Option{T}" /> without value.</returns>
   public static implicit operator Option<T> (NoneOption none) {
      _ = none;
      return default;
   }
}

public static partial class Option {
   public static NoneOption none => default;

   /// <returns>
   ///    An <see cref="Option{T}" /> with the specified <paramref name="value" />.
   /// </returns>
   public static Option<T> Some<T> (T value) {
      return new Option<T> { value = value };
   }

   /// <returns>The underlying value of <paramref name="option" />.</returns>
   /// <exception cref="AssertException"><paramref name="option" /> has no value.</exception>
   public static ref readonly T Value<T> (this in Option<T> option) {
      Assert.That(option.hasValue);
      return ref option._value!;
   }

   public static bool Is<T> (this in Option<T> option, [MaybeNullWhen(false)] out T value) {
      value = option._value;
      return option.hasValue;
   }

   public static void Deconstruct<T> (
      this in Option<T> option,
      out bool hasValue,
      out T valueOrDefault
   ) {
      hasValue = option.hasValue;
      valueOrDefault = option.value;
   }

   /// <param name="elseValue">
   ///    A value to return if <paramref name="option" /> has no value.
   /// </param>
   /// <returns>
   ///    The underlying value of <paramref name="option" /> if it exists;
   ///    otherwise, <paramref name="elseValue" />.
   /// </returns>
   public static ref readonly T Or<T> (this in Option<T> option, in T elseValue) {
      if (option.hasValue) {
         return ref option._value!;
      } else {
         return ref elseValue;
      }
   }

   /// <returns>
   ///    The underlying value of <paramref name="option" /> if it exists;
   ///    otherwise, default value of <typeparamref name="T" />.
   /// </returns>
   public static ref readonly T? OrDefault<T> (this in Option<T> option) {
      return ref option._value;
   }

   /// <returns>
   ///    The underlying value of <paramref name="option" /> if it exists;
   ///    otherwise, a result of invocation of <paramref name="elseFunc" />.
   /// </returns>
   public static T OrElse<T, TFunc> (
      this in Option<T> option,
      Pin<IFunctor<T>, TFunc> elseFunc
   ) where TFunc: IFunctor<T> {
      if (option.hasValue) {
         return option.value;
      } else {
         return elseFunc.Invoke();
      }
   }

   /// <returns>The underlying value of <paramref name="option" />.</returns>
   /// <exception cref="AssertException"><paramref name="option" />> has no value.</exception>
   public static ref readonly T OrFail<T> (this in Option<T> option, string message) {
      Assert.That(option.hasValue, message);
      return ref option._value!;
   }

   public static ref readonly Option<T> Coalesce<T> (this in Option<T> option, in Option<T> other) {
      if (option.hasValue) {
         return ref option;
      } else {
         return ref other;
      }
   }

   public static bool Equals<T> (this in Option<T> option, in Option<T> other) {
      if (option.hasValue) {
         return other.hasValue && EqualityComparer<T>.Default.Equals(option.value, other.value);
      } else {
         return !other.hasValue;
      }
   }
}

public readonly partial struct Option<T>: IEquatable<Option<T>> {
   public override int GetHashCode () {
      return this.hasValue ? this._value!.GetHashCode() : 0;
   }

   public bool Equals (Option<T> other) {
      return Option.Equals(in this, in other);
   }

   public override bool Equals (object? obj) {
      if (obj is not Option<T>) return false;

      ref readonly var other = ref Unsafe.Unbox<Option<T>>(obj);
      return this.Equals(in other);
   }

   public static bool operator == (in Option<T> left, in Option<T> right) {
      return Option.Equals(in left, in right);
   }

   public static bool operator != (in Option<T> left, in Option<T> right) {
      return !Option.Equals(in left, in right);
   }
}
