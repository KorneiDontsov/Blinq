using Blinq.Math;

namespace Blinq;

readonly struct AverageFoldFunc<T, TMath>: IFoldFunc<T, (T Sum, T Count)> where TMath: IMathOne<T>, IMathAdd<T> {
   readonly TMath Math;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AverageFoldFunc (TMath math) {
      Math = math;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (T Sum, T Count) accumulator) {
      accumulator.Sum = Math.Add(accumulator.Sum, accumulator.Count);
      accumulator.Count = Math.Add(accumulator.Count, Math.One());
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Average<T, TIterator, TMath> (this in Sequence<T, TIterator> sequence, TMath math)
   where TIterator: IIterator<T>
   where TMath: IMathZero<T>, IMathOne<T>, IMathAdd<T>, IMathDivide<T> {
      var (sum, count) = sequence.Iterator.Fold((Sum: math.One(), Count: math.Zero()), new AverageFoldFunc<T, TMath>(math));
      return math.Divide(sum, count);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Average<T, TIterator, TMath> (this in Sequence<T, TIterator> sequence, ProvideMath<T, TMath> provideMath)
   where TIterator: IIterator<T>
   where TMath: IMathZero<T>, IMathOne<T>, IMathAdd<T>, IMathDivide<T> {
      return sequence.Average(provideMath.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Average<TIterator> (this in Sequence<int, TIterator> sequence) where TIterator: IIterator<int> {
      return sequence.Average(new Int32Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static uint Average<TIterator> (this in Sequence<uint, TIterator> sequence) where TIterator: IIterator<uint> {
      return sequence.Average(new UInt32Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static long Average<TIterator> (this in Sequence<long, TIterator> sequence) where TIterator: IIterator<long> {
      return sequence.Average(new Int64Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ulong Average<TIterator> (this in Sequence<ulong, TIterator> sequence) where TIterator: IIterator<ulong> {
      return sequence.Average(new UInt64Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static decimal Average<TIterator> (this in Sequence<decimal, TIterator> sequence) where TIterator: IIterator<decimal> {
      return sequence.Average(new DecimalMath());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static float Average<TIterator> (this in Sequence<float, TIterator> sequence) where TIterator: IIterator<float> {
      return sequence.Average(new SingleFloatMath());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static double Average<TIterator> (this in Sequence<double, TIterator> sequence) where TIterator: IIterator<double> {
      return sequence.Average(new DoubleFloatMath());
   }
}
