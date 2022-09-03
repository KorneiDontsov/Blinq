using System.Collections.Generic;

namespace Blinq;

public interface ICompareCondition {
   bool Invoke (int compareResult);
}

public static class CompareConditions {
   public readonly struct Greater: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Invoke (int compareResult) {
         return compareResult > 0;
      }
   }

   public readonly struct Equal: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Invoke (int compareResult) {
         return compareResult == 0;
      }
   }

   public readonly struct Less: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Invoke (int compareResult) {
         return compareResult < 0;
      }
   }

   public readonly struct GreaterOrEqual: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Invoke (int compareResult) {
         return compareResult >= 0;
      }
   }

   public readonly struct LessOrEqual: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Invoke (int compareResult) {
         return compareResult <= 0;
      }
   }

   public readonly struct NotEqual: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public bool Invoke (int compareResult) {
         return compareResult != 0;
      }
   }
}

public static class CompareCondition {
   public static readonly CompareConditions.Greater Greater = default;
   public static readonly CompareConditions.Equal Equal = default;
   public static readonly CompareConditions.Less Less = default;
   public static readonly CompareConditions.GreaterOrEqual GreaterOrEqual = default;
   public static readonly CompareConditions.LessOrEqual LessOrEqual = default;
   public static readonly CompareConditions.NotEqual NotEqual = default;
}

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Compares<T, TCompareCondition, TComparer> (
      this T a,
      T b,
      TCompareCondition compareCondition,
      TComparer comparer
   )
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return compareCondition.Invoke(comparer.Compare(a, b));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Compares<T, TCompareCondition, TComparer> (
      this T a,
      T b,
      TCompareCondition compareCondition,
      ProvideComparer<T, TComparer> provideComparer
   )
   where TCompareCondition: ICompareCondition
   where TComparer: IComparer<T> {
      return Compares(a, b, compareCondition, provideComparer.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Compares<T, TCompareCondition> (
      this T a,
      T b,
      TCompareCondition compareCondition
   )
   where TCompareCondition: ICompareCondition {
      return Compares(a, b, compareCondition, Default<T>());
   }
}
