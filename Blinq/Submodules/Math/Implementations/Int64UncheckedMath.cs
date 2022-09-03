namespace Blinq.Math;

public readonly struct Int64UncheckedMath:
   IMathZero<long>,
   IMathOne<long>,
   IMathFrom<long, int>,
   IMathAdd<long>,
   IMathDivide<long> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long From (int n) {
      return n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long Add (long n1, long n2) {
      return n1 + n2;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long Divide (long n1, long n2) {
      return n1 / n2;
   }
}
