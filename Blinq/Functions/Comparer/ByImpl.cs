namespace Blinq;

public readonly struct ByImplComparer<T>: IComparer<T> where T: IComparable<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Compare (T? x, T? y) {
      return x switch {
         null => y switch {
            null => 0,
            not null => -1,
         },
         not null => y switch {
            null => 1,
            not null => x.CompareTo(y),
         },
      };
   }
}

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByImplComparer<T> ByImpl<T> () where T: IComparable<T> {
      return default;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByImplComparer<T> ByImpl<T> (this ComparerProvider<T> _) where T: IComparable<T> {
      return ByImpl<T>();
   }
}
