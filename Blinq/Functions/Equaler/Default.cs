using System.Collections.Generic;

namespace Blinq;

public readonly struct DefaultEqualer<T>: IEqualityComparer<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T? x, T? y) {
      return EqualityComparer<T>.Default.Equals(x!, y!);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode (T obj) {
      return EqualityComparer<T>.Default.GetHashCode(obj);
   }
}

public static partial class Equaler {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DefaultEqualer<T> Default<T> () {
      return default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DefaultEqualer<T> Default<T> (this EqualerProvider<T> _) {
      return Default<T>();
   }
}
