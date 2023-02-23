namespace Blinq;

readonly struct OptionalCount {
   const int NoValue = -1;

   readonly int Value;

   OptionalCount (int value) {
      Value = value;
   }

   public bool Is (out int count) {
      count = Value;
      return count > NoValue;
   }

   public static OptionalCount operator -- (OptionalCount count) {
      return count.Value > 0 ? new(count.Value - 1) : count;
   }

   public static OptionalCount Of<T> (IEnumerable<T> enumerable) {
      return new(
         enumerable switch {
            ICollection<T> collection => collection.Count,
            IReadOnlyCollection<T> collection => collection.Count,
            _ => NoValue,
         }
      );
   }

   public static OptionalCount None => new(NoValue);
}

/// <inheritdoc />
/// <summary>An <see cref="IEnumerator{T}" /> to <see cref="IIterator{T}" /> adapter.</summary>
public struct EnumeratorIterator<T>: IIterator<T> {
   readonly IEnumerator<T> Enumerator;
   OptionalCount Count;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal EnumeratorIterator (IEnumerator<T> enumerator, OptionalCount count) {
      Enumerator = enumerator;
      Count = count;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public EnumeratorIterator (IEnumerator<T> enumerator): this(enumerator, OptionalCount.None) { }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Enumerator.MoveNext()) {
         --Count;
         item = Enumerator.Current;
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<T, TAccumulator> {
      while (Enumerator.MoveNext()) {
         --Count;
         if (fold.Invoke(Enumerator.Current, ref seed)) break;
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      return Count.Is(out count);
   }
}

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public static partial class Iterator {
   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />.
   ///    WARNING: The sequence never dispose the underlying enumerator.
   ///    If you want the enumerator guaranteed to be disposed then use overloads of this method.
   /// </summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, EnumeratorIterator<T>> Iterate<T> (this IEnumerable<T> enumerable) {
      var count = OptionalCount.Of(enumerable);
      var enumerator = enumerable.GetEnumerator();
      return new EnumeratorIterator<T>(enumerator, count);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="action" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void Iterate<T> (this IEnumerable<T> enumerable, Action<Contract<IIterator<T>, EnumeratorIterator<T>>> action) {
      var count = OptionalCount.Of(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var iterator = new EnumeratorIterator<T>(enumerator, count);
      action(iterator);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="func" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Iterate<T, TResult> (this IEnumerable<T> enumerable, Func<Contract<IIterator<T>, EnumeratorIterator<T>>, TResult> func) {
      var count = OptionalCount.Of(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var iterator = new EnumeratorIterator<T>(enumerator, count);
      return func(iterator);
   }
}
