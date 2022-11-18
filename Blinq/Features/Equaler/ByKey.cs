namespace Blinq;

public readonly struct ByKeyEqualer<T, TKey, TKeyEqualer>: IEqualityComparer<T>
where TKey: notnull
where TKeyEqualer: IEqualityComparer<TKey> {
   readonly Func<T, TKey> SelectKey;
   readonly TKeyEqualer KeyEqualer;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ByKeyEqualer (Func<T, TKey> selectKey, TKeyEqualer keyEqualer) {
      SelectKey = selectKey;
      KeyEqualer = keyEqualer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T? x, T? y) {
      return KeyEqualer.Equals(SelectKey(x!), SelectKey(y!));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode (T obj) {
      return KeyEqualer.GetHashCode(SelectKey(obj));
   }
}

public static partial class Equaler {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, TKeyEqualer> ByKey<T, TKey, TKeyEqualer> (
      this EqualerProvider<T> equalerProvider,
      Func<T, TKey> selectKey,
      TKeyEqualer keyEqualer
   )
   where TKey: notnull
   where TKeyEqualer: IEqualityComparer<TKey> {
      _ = equalerProvider;
      return new ByKeyEqualer<T, TKey, TKeyEqualer>(selectKey, keyEqualer);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, TKeyEqualer> ByKey<T, TKey, TKeyEqualer> (
      this EqualerProvider<T> equalerProvider,
      Func<T, TKey> selectKey,
      ProvideEqualer<TKey, TKeyEqualer> provideKeyEqualer
   )
   where TKey: notnull
   where TKeyEqualer: IEqualityComparer<TKey> {
      return equalerProvider.ByKey(selectKey, provideKeyEqualer.Invoke());
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, DefaultEqualer<TKey>> ByKey<T, TKey> (this EqualerProvider<T> equalerProvider, Func<T, TKey> selectKey)
   where TKey: notnull {
      return equalerProvider.ByKey(selectKey, Get<TKey>.Equaler.Default());
   }
}
