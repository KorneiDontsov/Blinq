namespace Blinq;

public readonly struct ByImplEqualer<T>: IEqualityComparer<T> where T: IEquatable<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T? x, T? y) {
      var (left, right) = x is null ? (y, x) : (x, y);
      return left?.Equals(right) ?? true;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode (T obj) {
      return obj.GetHashCode();
   }
}

public static partial class Equaler {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByImplEqualer<T> ByImpl<T> (this EqualerProvider<T> equalerProvider) where T: IEquatable<T> {
      _ = equalerProvider;
      return new();
   }
}
