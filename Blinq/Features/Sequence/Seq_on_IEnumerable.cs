namespace Blinq;

/// <inheritdoc />
/// <summary>An <see cref="IEnumerator{T}" /> to <see cref="IIterator{T}" /> adapter.</summary>
[SuppressMessage("ReSharper", "StructCanBeMadeReadOnly")]
public readonly struct EnumeratorIterator<T>: IIterator<T> {
   readonly IEnumerator<T> Enumerator;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public EnumeratorIterator (IEnumerator<T> enumerator) {
      Enumerator = enumerator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      while (Enumerator.MoveNext() && !func.Invoke(Enumerator.Current, ref seed)) { }

      return seed;
   }
}

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
public static partial class Sequence {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static Option<int> GetCount<T> (IEnumerable<T> enumerable) {
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
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, EnumeratorIterator<T>> Seq<T> (this IEnumerable<T> enumerable) {
      var count = GetCount(enumerable);
      return Sequence<T>.Create(new EnumeratorIterator<T>(enumerable.GetEnumerator()), count);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="action" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void Seq<T> (this IEnumerable<T> enumerable, Action<Sequence<T, EnumeratorIterator<T>>> action) {
      var count = GetCount(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var sequence = Sequence<T>.Create(new EnumeratorIterator<T>(enumerator), count);
      action(sequence);
   }

   /// <summary>
   ///    Creates a sequence over <paramref name="enumerable" />,
   ///    then executes <paramref name="func" /> with it,
   ///    then disposes the underlying enumerator.
   /// </summary>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TResult Seq<T, TResult> (this IEnumerable<T> enumerable, Func<Sequence<T, EnumeratorIterator<T>>, TResult> func) {
      var count = GetCount(enumerable);
      using var enumerator = enumerable.GetEnumerator();
      var sequence = Sequence<T>.Create(new EnumeratorIterator<T>(enumerator), count);
      return func(sequence);
   }
}