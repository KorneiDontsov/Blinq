namespace Blinq;

public readonly struct ByImplComparer<T>: IComparer<T> where T: IComparable<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Compare (T? x, T? y) {
      var (left, right) = x is null ? (y, x) : (x, y);
      return left?.CompareTo(right) ?? 0;
   }
}

public static partial class Comparers {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByImplComparer<T> ByImpl<T> (this ComparerProvider<T> comparerProvider) where T: IComparable<T> {
      _ = comparerProvider;
      return new();
   }
}
