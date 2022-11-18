namespace Blinq;

public readonly struct ByRefEqualer<T>: IEqualityComparer<T> where T: class {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T? x, T? y) {
      return ReferenceEquals(x, y);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode (T obj) {
      return RuntimeHelpers.GetHashCode(obj);
   }
}

public static partial class Equaler {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByRefEqualer<T> ByRef<T> (this EqualerProvider<T> equalerProvider) where T: class {
      _ = equalerProvider;
      return new();
   }
}
