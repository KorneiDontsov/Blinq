namespace Blinq.Math;

public readonly struct Int32Math:
   IMathZero<int>,
   IMathOne<int>,
   IMathFrom<int, int>,
   IMathAdd<int>,
   IMathDivide<int> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int One () {
      return 1;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int From (int n) {
      return n;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Add (int n1, int n2) {
      return checked(n1 + n2);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Divide (int n1, int n2) {
      return n1 / n2;
   }
}
