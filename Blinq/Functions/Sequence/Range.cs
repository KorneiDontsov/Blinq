using Blinq.Math;

namespace Blinq;

public struct RangeIterator<T, TMath>: IIterator<T> where TMath: IMathOne<T>, IMathAdd<T> {
   readonly TMath Math;
   T Current;
   int CountLeft;
   bool Started;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RangeIterator (T start, int count, TMath math) {
      Math = math;
      Current = start;
      CountLeft = count;
      Started = count == 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      if (!Started) {
         Started = true;
         if (func.Invoke(Current, ref seed)) return seed;
      }

      while (--CountLeft > 0) {
         Current = Math.Add(Current, Math.One());
         if (func.Invoke(Current, ref seed)) return seed;
      }

      return seed;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, RangeIterator<T, TMath>> Range<T, TMath> (T start, int count, TMath math) where TMath: IMathOne<T>, IMathAdd<T> {
      if (count < 0) Utils.Throw<ArgumentOutOfRangeException>();

      return Sequence<T>.Create(new RangeIterator<T, TMath>(start, count, math), count);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, RangeIterator<T, TMath>> Range<T, TMath> (T start, int count, ProvideMath<T, TMath> provideMath)
   where TMath: IMathOne<T>, IMathAdd<T> {
      return Range(start, count, provideMath.Invoke());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<int, RangeIterator<int, Int32Math>> Range (int start, int count) {
      return Range(start, count, new Int32Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<uint, RangeIterator<uint, UInt32Math>> Range (uint start, int count) {
      return Range(start, count, new UInt32Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<long, RangeIterator<long, Int64Math>> Range (long start, int count) {
      return Range(start, count, new Int64Math());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<ulong, RangeIterator<ulong, UInt64Math>> Range (ulong start, int count) {
      return Range(start, count, new UInt64Math());
   }
}
