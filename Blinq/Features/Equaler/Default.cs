namespace Blinq;

public readonly struct DefaultEqualer<T>: IEqualityComparer<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T? x, T? y) {
      return EqualityComparer<T>.Default.Equals(x, y);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode ([DisallowNull] T obj) {
      return EqualityComparer<T>.Default.GetHashCode(obj);
   }
}

public static partial class Equaler {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DefaultEqualer<T> Default<T> (this EqualerProvider<T> equalerProvider) {
      _ = equalerProvider;
      return new();
   }
}
