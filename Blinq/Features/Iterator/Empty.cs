namespace Blinq;

public readonly struct EmptyIterator<T>: IIterator<T> {
   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      item = default;
      return false;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<T, TAccumulator> {
      return accumulator;
   }

   public bool TryGetCount (out int count) {
      count = 0;
      return true;
   }
}

public static partial class Iterator {
   /// <summary>Returns empty sequence of a specified type.</summary>
   /// <typeparam name="T">The type of elements of a sequence.</typeparam>
   /// <returns>Empty sequence.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, EmptyIterator<T>> Empty<T> () {
      return new EmptyIterator<T>();
   }
}
