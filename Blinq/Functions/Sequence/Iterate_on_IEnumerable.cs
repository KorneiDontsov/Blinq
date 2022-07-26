using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

/// <inheritdoc />
/// <summary>An <see cref="IEnumerator{T}" /> to <see cref="IIterator{T}" /> adapter.</summary>
[SuppressMessage("ReSharper", "StructCanBeMadeReadOnly")]
public struct EnumeratorIterator<T>: IIterator<T> {
   readonly IEnumerator<T> Enumerator;

   public EnumeratorIterator (IEnumerator<T> enumerator) {
      Enumerator = enumerator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<T, TAccumulated> {
      while (Enumerator.MoveNext() && !accumulator.Invoke(Enumerator.Current, ref seed)) { }

      return seed;
   }
}

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public static partial class Sequence {
   static Option<int> GetCountOrDefault<T> (IEnumerable<T> enumerable) {
      return enumerable switch {
         ICollection<T> collection => collection.Count,
         IReadOnlyCollection<T> collection => collection.Count,
         _ => Option.None,
      };
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />.
   ///    WARNING: The sequence never dispose the underlying enumerator.
   ///    If you want the enumerator guaranteed to be disposed then use overloads of this method.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, EnumeratorIterator<T>> Iterate<T> (this IEnumerable<T> enumerable) {
      var count = GetCountOrDefault(enumerable);
      return new Sequence<T, EnumeratorIterator<T>>(new EnumeratorIterator<T>(enumerable.GetEnumerator()), count);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="action" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void Iterate<T> (this IEnumerable<T> enumerable, Action<Sequence<T, EnumeratorIterator<T>>> action) {
      var count = GetCountOrDefault(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var sequence = new Sequence<T, EnumeratorIterator<T>>(new EnumeratorIterator<T>(enumerator), count);
      action(sequence);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="func" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Iterate<T, TResult> (this IEnumerable<T> enumerable, Func<Sequence<T, EnumeratorIterator<T>>, TResult> func) {
      var count = GetCountOrDefault(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var sequence = new Sequence<T, EnumeratorIterator<T>>(new EnumeratorIterator<T>(enumerator), count);
      return func(sequence);
   }
}
