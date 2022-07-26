using System.Runtime.CompilerServices;

namespace Blinq;

interface ISumTrait<T> {
   T Zero ();
   T Sum (T a, T b);
}

readonly struct Int32SumTrait: ISumTrait<int> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public int Sum (int a, int b) {
      return a + b;
   }
}

readonly struct Int64SumTrait: ISumTrait<long> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long Zero () {
      return 0;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public long Sum (long a, long b) {
      return a + b;
   }
}

readonly struct SumAccumulator<T, TSumTrait>: IAccumulator<T, T> where TSumTrait: ISumTrait<T> {
   readonly TSumTrait SumTrait;

   public SumAccumulator (TSumTrait sumTrait) {
      SumTrait = sumTrait;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref T accumulated) {
      accumulated = SumTrait.Sum(accumulated, item);
      return false;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static T Sum<T, TIterator, TSumTrait> (this in Sequence<T, TIterator> sequence, TSumTrait sumTrait)
   where TIterator: IIterator<T>
   where TSumTrait: ISumTrait<T> {
      return sequence.Iterator.Accumulate(new SumAccumulator<T, TSumTrait>(sumTrait), sumTrait.Zero());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Sum<TIterator> (this in Sequence<int, TIterator> sequence) where TIterator: IIterator<int> {
      return sequence.Sum(new Int32SumTrait());
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static long Sum<TIterator> (this in Sequence<long, TIterator> sequence) where TIterator: IIterator<long> {
      return sequence.Sum(new Int64SumTrait());
   }
}
