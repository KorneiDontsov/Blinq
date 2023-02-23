namespace Blinq;

[ReadOnly(true)]
public interface ICompareCondition {
   [Pure] static abstract bool Compares (int compareResult);
}

public static class CompareConditions {
   public sealed class Greater: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool Compares (int compareResult) {
         return compareResult > 0;
      }
   }

   public sealed class Equal: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool Compares (int compareResult) {
         return compareResult == 0;
      }
   }

   public sealed class Less: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool Compares (int compareResult) {
         return compareResult < 0;
      }
   }

   public sealed class GreaterOrEqual: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool Compares (int compareResult) {
         return compareResult >= 0;
      }
   }

   public sealed class LessOrEqual: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool Compares (int compareResult) {
         return compareResult <= 0;
      }
   }

   public sealed class NotEqual: ICompareCondition {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static bool Compares (int compareResult) {
         return compareResult != 0;
      }
   }
}

public static class CompareCondition {
   public static Type<CompareConditions.Greater> Greater => default;
   public static Type<CompareConditions.Equal> Equal => default;
   public static Type<CompareConditions.Less> Less => default;
   public static Type<CompareConditions.GreaterOrEqual> GreaterOrEqual => default;
   public static Type<CompareConditions.LessOrEqual> LessOrEqual => default;
   public static Type<CompareConditions.NotEqual> NotEqual => default;
}

public static partial class Comparers {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Compares<T, TCondition, TComparer> (
      this T a,
      T b,
      TComparer comparer,
      Type<TCondition> tCondition = default
   )
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      _ = tCondition;
      return TCondition.Compares(comparer.Compare(a, b));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Compares<T, TCondition, TComparer> (
      this T a,
      T b,
      ProvideComparer<T, TComparer> provideComparer,
      Type<TCondition> tCondition = default
   )
   where TComparer: IComparer<T>
   where TCondition: ICompareCondition {
      return Compares(a, b, provideComparer.Invoke(), tCondition);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool Compares<T, TCondition> (
      this T a,
      T b,
      Type<TCondition> tCondition = default
   )
   where TCondition: ICompareCondition {
      return Compares(a, b, Get<T>.Comparer.Default(), tCondition);
   }
}
