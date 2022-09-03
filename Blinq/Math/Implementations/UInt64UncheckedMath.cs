namespace Blinq.Math;

public readonly struct UInt64UncheckedMath:
   IMathZero<ulong>,
   IMathOne<ulong>,
   IMathFrom<ulong, int>,
   IMathAdd<ulong>,
   IMathDivide<ulong> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ulong Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ulong One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ulong From (int n) {
      return (ulong)n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ulong Add (ulong n1, ulong n2) {
      return n1 + n2;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ulong Divide (ulong n1, ulong n2) {
      return n1 / n2;
   }
}
