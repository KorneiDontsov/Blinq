namespace Blinq;

public readonly struct DefaultComparer<T>: IComparer<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Compare (T x, T y) {
      return Comparer<T>.Default.Compare(x, y);
   }
}

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DefaultComparer<T> Default<T> () {
      return default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DefaultComparer<T> Default<T> (this ComparerProvider<T> _) {
      return Default<T>();
   }
}
