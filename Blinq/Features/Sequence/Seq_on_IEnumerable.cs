namespace Blinq;

/// <inheritdoc />
/// <summary>An <see cref="IEnumerator{T}" /> to <see cref="IIterator{T}" /> adapter.</summary>
public struct EnumeratorIterator<T>: IIterator<T> {
   readonly IEnumerator<T> Enumerator;
   int Count;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal EnumeratorIterator (IEnumerator<T> enumerator, int count) {
      Enumerator = enumerator;
      Count = count;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public EnumeratorIterator (IEnumerator<T> enumerator): this(enumerator, count: -1) { }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      if (Enumerator.MoveNext()) {
         if (Count > 0) --Count;
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
         if (Count > 0) --Count;
         if (fold.Invoke(Enumerator.Current, ref seed)) break;
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = Count;
      return count >= 0;
   }
}

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static int GetCount<T> (IEnumerable<T> enumerable) {
      return enumerable switch {
         ICollection<T> collection => collection.Count,
         IReadOnlyCollection<T> collection => collection.Count,
         _ => -1,
      };
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />.
   ///    WARNING: The sequence never dispose the underlying enumerator.
   ///    If you want the enumerator guaranteed to be disposed then use overloads of this method.
   /// </summary>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, EnumeratorIterator<T>> Iter<T> (this IEnumerable<T> enumerable) {
      var count = GetCount(enumerable);
      var enumerator = enumerable.GetEnumerator();
      return new EnumeratorIterator<T>(enumerator, count);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="action" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void Iter<T> (this IEnumerable<T> enumerable, Action<Contract<IIterator<T>, EnumeratorIterator<T>>> action) {
      var count = GetCount(enumerable);
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
   public static TResult Iter<T, TResult> (this IEnumerable<T> enumerable, Func<Contract<IIterator<T>, EnumeratorIterator<T>>, TResult> func) {
      var count = GetCount(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var iterator = new EnumeratorIterator<T>(enumerator, count);
      return func(iterator);
   }
}
