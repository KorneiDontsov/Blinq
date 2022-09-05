using Blinq.Math;

namespace Blinq;

readonly struct SumFoldFunc<T, TMath>: IFoldFunc<T, T> where TMath: IMathAdd<T> {
   readonly TMath Math;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public SumFoldFunc (TMath math) {
      Math = math;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulator) {
      accumulator = Math.Add(accumulator, item);
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Sum<T, TIterator, TMath> (this in Sequence<T, TIterator> sequence, TMath math)
   where TIterator: IIterator<T>
   where TMath: IMathZero<T>, IMathAdd<T> {
      return sequence.Iterator.Fold(math.Zero(), new SumFoldFunc<T, TMath>(math));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static T Sum<T, TIterator, TMath> (this in Sequence<T, TIterator> sequence, ProvideMath<T, TMath> provideMath)
   where TIterator: IIterator<T>
   where TMath: IMathZero<T>, IMathAdd<T> {
      return sequence.Sum(provideMath.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Sum<TIterator> (this in Sequence<int, TIterator> sequence) where TIterator: IIterator<int> {
      return sequence.Sum(new Int32Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static uint Sum<TIterator> (this in Sequence<uint, TIterator> sequence) where TIterator: IIterator<uint> {
      return sequence.Sum(new UInt32Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static long Sum<TIterator> (this in Sequence<long, TIterator> sequence) where TIterator: IIterator<long> {
      return sequence.Sum(new Int64Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ulong Sum<TIterator> (this in Sequence<ulong, TIterator> sequence) where TIterator: IIterator<ulong> {
      return sequence.Sum(new UInt64Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static decimal Sum<TIterator> (this in Sequence<decimal, TIterator> sequence) where TIterator: IIterator<decimal> {
      return sequence.Sum(new DecimalMath());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static float Sum<TIterator> (this in Sequence<float, TIterator> sequence) where TIterator: IIterator<float> {
      return sequence.Sum(new SingleFloatMath());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static double Sum<TIterator> (this in Sequence<double, TIterator> sequence) where TIterator: IIterator<double> {
      return sequence.Sum(new DoubleFloatMath());
   }
}
