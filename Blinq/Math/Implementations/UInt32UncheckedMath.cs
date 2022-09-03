namespace Blinq.Math;

public readonly struct UInt32UncheckedMath:
   IMathZero<uint>,
   IMathOne<uint>,
   IMathFrom<uint, int>,
   IMathAdd<uint>,
   IMathDivide<uint> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public uint Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public uint One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public uint From (int n) {
      return (uint)n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public uint Add (uint n1, uint n2) {
      return n1 + n2;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public uint Divide (uint n1, uint n2) {
      return n1 / n2;
   }
}
