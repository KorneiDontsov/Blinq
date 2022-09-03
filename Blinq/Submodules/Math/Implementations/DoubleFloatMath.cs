namespace Blinq.Math;

public readonly struct DoubleFloatMath:
   IMathZero<double>,
   IMathOne<double>,
   IMathFrom<double, int>,
   IMathAdd<double>,
   IMathDivide<double> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public double Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public double One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public double From (int n) {
      return n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public double Add (double n1, double n2) {
      return n1 + n2;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public double Divide (double n1, double n2) {
      return n1 / n2;
   }
}
