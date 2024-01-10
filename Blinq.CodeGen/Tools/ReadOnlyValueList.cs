using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Blinq.CodeGen;

readonly struct ReadOnlyValueList<T>: IReadOnlyList<T>, IEquatable<ReadOnlyValueList<T>> {
   public required T[] array { private get; init; }

   public int Count => array.Length;
   public T this [int index] => array[index];

   public static implicit operator ReadOnlyValueList<T> (T[] array) {
      return new() { array = array };
   }

   public static implicit operator ReadOnlyValueList<T> (ImmutableArray<T> array) {
      return Unsafe.As<ImmutableArray<T>, ReadOnlyValueList<T>>(ref array);
   }

   public static ReadOnlyValueList<T> empty => Array.Empty<T>();

   public ReadOnlySpan<T> AsSpan () {
      return new ReadOnlySpan<T>(array);
   }

   public ReadOnlySpan<T>.Enumerator GetEnumerator () {
      return AsSpan().GetEnumerator();
   }

   IEnumerator<T> IEnumerable<T>.GetEnumerator () {
      return ((IEnumerable<T>)array).GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator () {
      return array.GetEnumerator();
   }

   public bool Equals (ReadOnlyValueList<T> other) {
      if (array.Length != other.array.Length) return false;

      for (var index = 0; index < array.Length; ++index) {
         var areEqual = EqualityComparer<T>.Default.Equals(array[index], other.array[index]);
         if (!areEqual) return false;
      }

      return true;
   }

   public override bool Equals (object? obj) {
      return obj is ReadOnlyValueList<T> other && Equals(other);
   }

   public override int GetHashCode () {
      return array.GetHashCode();
   }

   public ReadOnlyValueList<TResult> ConvertAll<TResult> (Func<T, TResult> selector) {
      return array.ConvertAll(selector);
   }
}
