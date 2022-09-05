namespace Blinq;

public readonly struct ByKeyEqualer<T, TKey, TKeyEqualer>: IEqualityComparer<T> where TKeyEqualer: IEqualityComparer<TKey> {
   readonly Func<T, TKey> SelectKey;
   readonly TKeyEqualer KeyEqualer;

   public ByKeyEqualer (Func<T, TKey> selectKey, TKeyEqualer keyEqualer) {
      SelectKey = selectKey;
      KeyEqualer = keyEqualer;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Equals (T x, T y) {
      return KeyEqualer.Equals(SelectKey(x), SelectKey(y));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int GetHashCode (T obj) {
      return KeyEqualer.GetHashCode(SelectKey(obj));
   }
}

public static partial class Equaler {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, TKeyEqualer> ByKey<T, TKey, TKeyEqualer> (Func<T, TKey> selectKey, TKeyEqualer keyEqualer)
   where TKeyEqualer: IEqualityComparer<TKey> {
      return new ByKeyEqualer<T, TKey, TKeyEqualer>(selectKey, keyEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, TKeyEqualer> ByKey<T, TKey, TKeyEqualer> (
      this EqualerProvider<T> _,
      Func<T, TKey> selectKey,
      TKeyEqualer keyEqualer
   )
   where TKeyEqualer: IEqualityComparer<TKey> {
      return ByKey(selectKey, keyEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, TKeyEqualer> ByKey<T, TKey, TKeyEqualer> (
      Func<T, TKey> selectKey,
      ProvideEqualer<TKey, TKeyEqualer> provideKeyEqualer
   )
   where TKeyEqualer: IEqualityComparer<TKey> {
      return ByKey(selectKey, provideKeyEqualer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, TKeyEqualer> ByKey<T, TKey, TKeyEqualer> (
      this EqualerProvider<T> _,
      Func<T, TKey> selectKey,
      ProvideEqualer<TKey, TKeyEqualer> provideKeyEqualer
   )
   where TKeyEqualer: IEqualityComparer<TKey> {
      return ByKey(selectKey, provideKeyEqualer);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, DefaultEqualer<TKey>> ByKey<T, TKey> (Func<T, TKey> selectKey) {
      return new ByKeyEqualer<T, TKey, DefaultEqualer<TKey>>(selectKey, Default<TKey>());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ByKeyEqualer<T, TKey, DefaultEqualer<TKey>> ByKey<T, TKey> (this EqualerProvider<T> _, Func<T, TKey> selectKey) {
      return ByKey(selectKey);
   }
}
