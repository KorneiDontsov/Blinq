namespace Blinq;

public readonly struct EmptyIterator<T>: IIterator<T> {
   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return seed;
   }
}

public static partial class Sequence {
   /// <summary>Returns empty sequence of a specified type.</summary>
   /// <typeparam name="T">The type of elements of a sequence.</typeparam>
   /// <returns>Empty sequence.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, EmptyIterator<T>> Empty<T> () {
      return Sequence<T>.Create(new EmptyIterator<T>(), count: 0);
   }
}
