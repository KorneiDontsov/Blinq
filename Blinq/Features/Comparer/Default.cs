namespace Blinq;

public readonly struct DefaultComparer<T>: IComparer<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Compare (T? x, T? y) {
      return Comparer<T>.Default.Compare(x, y);
   }
}

public static partial class Comparers {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DefaultComparer<T> Default<T> (this ComparerProvider<T> _) {
      return new();
   }
}
