using System.Collections.Generic;

namespace Blinq;

public readonly struct ByImplEqualer<T>: IEqualityComparer<T> where T: IEquatable<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T? x, T? y) {
      return x?.Equals(y!) ?? y?.Equals(x!) ?? false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode (T obj) {
      return obj.GetHashCode();
   }
}

public static partial class Equaler {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByImplEqualer<T> ByImpl<T> () where T: IEquatable<T> {
      return default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByImplEqualer<T> ByImpl<T> (this EqualerProvider<T> _) where T: IEquatable<T> {
      return ByImpl<T>();
   }
}
