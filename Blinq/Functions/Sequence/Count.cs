using Blinq.Math;

namespace Blinq;

struct CountFoldFunc<T, TCounter, TMath>: IFoldFunc<T, TCounter> where TMath: IMathOne<TCounter>, IMathAdd<TCounter> {
   TMath Math;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public CountFoldFunc (TMath math) {
      Math = math;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TCounter accumulator) {
      accumulator = Math.Add(accumulator, Math.One());
      return false;
   }
}

public static partial class Sequence {
   public static TCounter Count<T, TIterator, TCounter, TMath> (this in Sequence<T, TIterator> sequence, Use<TCounter, TMath> mathUse)
   where TIterator: IIterator<T>
   where TMath: IMathZero<TCounter>, IMathOne<TCounter>, IMathFrom<TCounter, int>, IMathAdd<TCounter> {
      var math = mathUse.Contract;
      return sequence.Count switch {
         (true, var count) => math.From(count),
         _ => sequence.Iterator.Fold(math.Zero(), new CountFoldFunc<T, TCounter, TMath>(math)),
      };
   }

   /// <summary>Returns the number of elements in a sequence.</summary>
   /// <returns>The number of elements in <paramref name="sequence" />.</returns>
   /// <exception cref="OverflowException">
   ///    The number of elements in <paramref name="sequence" /> is larger than <see cref="Int32.MaxValue" />.
   /// </exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static int Count<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Count(new Use<int, Int32Math>(new Int32Math()));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static long LongCount<T, TIterator> (this in Sequence<T, TIterator> sequence) where TIterator: IIterator<T> {
      return sequence.Count(new Use<long, Int64Math>(new Int64Math()));
   }
}
