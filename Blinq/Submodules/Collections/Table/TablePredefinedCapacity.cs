using System.Runtime.InteropServices;

namespace Blinq.Collections;

[StructLayout(LayoutKind.Sequential)]
struct TablePredefinedCapacity {
   const int MaxValue = 1164186217;

   static readonly int[] Values = {
      5,
      11,
      23,
      47,
      97,
      199,
      409,
      823,
      1741,
      3469,
      6949,
      14033,
      28411,
      57557,
      116731,
      236897,
      480881,
      976369,
      1982627,
      4026031,
      8175383,
      16601593,
      33712729,
      68460391,
      139022417,
      282312799,
      573292817,
      MaxValue,
   };

   public static TablePredefinedCapacity None { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default; }

   int Number;

   public int Apply (int minCapacity) {
      if ((uint)minCapacity < MaxValue) {
         var number = 1;
         foreach (var value in Values) {
            if (value >= minCapacity) {
               Number = number;
               return value;
            }

            ++number;
         }
      }

      Number = 0;
      return int.MaxValue;
   }

   public int Grow (int currentCapacity) {
      if (Number > 0 && Number < Values.Length) {
         return Values[Number++];
      } else {
         return Apply(minCapacity: currentCapacity * 2);
      }
   }
}
