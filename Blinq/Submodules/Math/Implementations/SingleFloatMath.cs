namespace Blinq.Math;

public readonly struct SingleFloatMath:
   IMathZero<float>,
   IMathOne<float>,
   IMathFrom<float, int>,
   IMathAdd<float>,
   IMathDivide<float> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public float Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public float One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public float From (int n) {
      return n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public float Add (float n1, float n2) {
      return n1 + n2;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public float Divide (float n1, float n2) {
      return n1 / n2;
   }
}
