namespace Blinq.Math;

public readonly struct DecimalMath:
   IMathZero<decimal>,
   IMathOne<decimal>,
   IMathFrom<decimal, int>,
   IMathAdd<decimal>,
   IMathDivide<decimal> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public decimal Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public decimal One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public decimal From (int n) {
      return n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public decimal Add (decimal n1, decimal n2) {
      return n1 + n2;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public decimal Divide (decimal n1, decimal n2) {
      return n1 / n2;
   }
}
