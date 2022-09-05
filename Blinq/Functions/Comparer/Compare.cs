namespace Blinq;

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Compare<T, TComparer> (T a, T b, ProvideComparer<T, TComparer> provideComparer) where TComparer: IComparer<T> {
      var comparer = provideComparer.Invoke();
      return comparer.Compare(a, b);
   }
}
