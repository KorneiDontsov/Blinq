namespace Blinq.Math;

public readonly struct MathProvider<T> { }

// ReSharper disable once TypeParameterCanBeVariant
public delegate TMath ProvideMath<T, TMath> (MathProvider<T> mathProvider) where TMath: IMath<T>;

public static class MathProviding {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TMath Invoke<T, TMath> (this ProvideMath<T, TMath> provideMath) where TMath: IMath<T> {
      return provideMath(new MathProvider<T>());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Int32Math Default (this MathProvider<int> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static UInt32Math Default (this MathProvider<uint> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Int64Math Default (this MathProvider<long> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static UInt64Math Default (this MathProvider<ulong> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DecimalMath Default (this MathProvider<decimal> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static SingleFloatMath Default (this MathProvider<float> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static DoubleFloatMath Default (this MathProvider<double> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Int32UncheckedMath Unchecked (this MathProvider<int> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static UInt32UncheckedMath Unchecked (this MathProvider<uint> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Int64UncheckedMath Unchecked (this MathProvider<long> _) {
      return new();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static UInt64UncheckedMath Unchecked (this MathProvider<ulong> _) {
      return new();
   }
}
