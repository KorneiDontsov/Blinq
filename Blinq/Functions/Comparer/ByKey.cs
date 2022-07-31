using System.Collections.Generic;

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
   public int Compare (T x, T y) {
      return KeyComparer.Compare(SelectKey(x), SelectKey(y));
   }
}

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyComparer<T, TKey, TKeyComparer> ByKey<T, TKey, TKeyComparer> (Func<T, TKey> selectKey, TKeyComparer keyComparer)
   where TKeyComparer: IComparer<TKey> {
      return new ByKeyComparer<T, TKey, TKeyComparer>(selectKey, keyComparer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyComparer<T, TKey, TKeyComparer> ByKey<T, TKey, TKeyComparer> (
      Func<T, TKey> selectKey,
      Func<ComparerProvider<TKey>, TKeyComparer> provideKeyComparer
   )
   where TKeyComparer: IComparer<TKey> {
      return ByKey(selectKey, provideKeyComparer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyComparer<T, TKey, DefaultComparer<TKey>> ByKey<T, TKey> (Func<T, TKey> selectKey) {
      return ByKey(selectKey, Default<TKey>());
   }
}
