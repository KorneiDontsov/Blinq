namespace Blinq;

public readonly struct ByKeyComparer<T, TKey, TKeyComparer>: IComparer<T> where TKeyComparer: IComparer<TKey> {
   readonly Func<T, TKey> SelectKey;
   readonly TKeyComparer KeyComparer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ByKeyComparer (Func<T, TKey> selectKey, TKeyComparer keyComparer) {
      SelectKey = selectKey;
      KeyComparer = keyComparer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Compare (T? x, T? y) {
      return KeyComparer.Compare(SelectKey(x!), SelectKey(y!));
   }
}

public static partial class Comparers {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyComparer<T, TKey, TKeyComparer> ByKey<T, TKey, TKeyComparer> (
      this ComparerProvider<T> comparerProvider,
      Func<T, TKey> selectKey,
      TKeyComparer keyComparer
   )
   where TKeyComparer: IComparer<TKey> {
      _ = comparerProvider;
      return new ByKeyComparer<T, TKey, TKeyComparer>(selectKey, keyComparer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyComparer<T, TKey, TKeyComparer> ByKey<T, TKey, TKeyComparer> (
      this ComparerProvider<T> comparerProvider,
      Func<T, TKey> selectKey,
      ProvideComparer<TKey, TKeyComparer> provideKeyComparer
   )
   where TKeyComparer: IComparer<TKey> {
      return comparerProvider.ByKey(selectKey, provideKeyComparer.Invoke());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyComparer<T, TKey, DefaultComparer<TKey>> ByKey<T, TKey> (this ComparerProvider<T> comparerProvider, Func<T, TKey> selectKey) {
      return comparerProvider.ByKey(selectKey, Get<TKey>.Comparer.Default());
   }
}