namespace Blinq;

struct ZipFoldFunc<TIn1, TAccumulator, TIn2, TOut, TZipper, TIn2Iterator, TInnerFoldFunc>:
   IFoldFunc<TIn1, (TAccumulator accumulator, TIn2Iterator iterator2)>
where TIn2Iterator: IIterator<TIn2>
where TZipper: IItemZipper<TIn1, TIn2, TOut>
where TInnerFoldFunc: IFoldFunc<TOut, TAccumulator> {
   TZipper Zipper;
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ZipFoldFunc (TZipper zipper, TInnerFoldFunc innerFoldFunc) {
      Zipper = zipper;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn1 item1, ref (TAccumulator accumulator, TIn2Iterator iterator2) state) {
      return Sequence<TIn2>.Pop(ref state.iterator2) is (true, var item2)
         && InnerFoldFunc.Invoke(Zipper.Invoke(item1, item2), ref state.accumulator);
   }
}

public struct ZipIterator<TOut, TIn1, TIn2, TZipper, TIn1Iterator, TIn2Iterator>: IIterator<TOut>
where TIn1Iterator: IIterator<TIn1>
where TIn2Iterator: IIterator<TIn2>
where TZipper: IItemZipper<TIn1, TIn2, TOut> {
   TIn1Iterator Iterator1;
   TIn2Iterator Iterator2;
   readonly TZipper Zipper;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ZipIterator (TIn1Iterator iterator1, TIn2Iterator iterator2, TZipper zipper) {
      Iterator1 = iterator1;
      Iterator2 = iterator2;
      Zipper = zipper;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      (seed, Iterator2) =
         Iterator1.Fold((seed, Iterator2), new ZipFoldFunc<TIn1, TAccumulator, TIn2, TOut, TZipper, TIn2Iterator, TFoldFunc>(Zipper, func));
      return seed;
   }
}

public static partial class Sequence {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, ZipIterator<TResult, T1, T2, TZipper, T1Iterator, T2Iterator>>
      Zip<T1, T1Iterator, T2, T2Iterator, TResult, TZipper> (
         this in Sequence<T1, T1Iterator> sequence1,
         Sequence<T2, T2Iterator> sequence2,
         TZipper zipper
      )
   where T1Iterator: IIterator<T1>
   where T2Iterator: IIterator<T2>
   where TZipper: IItemZipper<T1, T2, TResult> {
      var count =
         (sequence1.Count, sequence2.Count) switch {
            ((true, var count1), (true, var count2)) => Option.Value(System.Math.Min(count1, count2)),
            _ => Option.None,
         };
      var iterator = new ZipIterator<TResult, T1, T2, TZipper, T1Iterator, T2Iterator>(sequence1.Iterator, sequence2.Iterator, zipper);
      return new Sequence<TResult, ZipIterator<TResult, T1, T2, TZipper, T1Iterator, T2Iterator>>(iterator, count);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<TResult, ZipIterator<TResult, T1, T2, FuncItemZipper<T1, T2, TResult>, T1Iterator, T2Iterator>>
      Zip<T1, T1Iterator, T2, T2Iterator, TResult> (
         this in Sequence<T1, T1Iterator> sequence1,
         Sequence<T2, T2Iterator> sequence2,
         Func<T1, T2, TResult> zipper
      )
   where T1Iterator: IIterator<T1>
   where T2Iterator: IIterator<T2> {
      return sequence1.Zip<T1, T1Iterator, T2, T2Iterator, TResult, FuncItemZipper<T1, T2, TResult>>(
         sequence2,
         new FuncItemZipper<T1, T2, TResult>(zipper)
      );
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<(T1, T2), ZipIterator<(T1, T2), T1, T2, TupleItemZipper<T1, T2>, T1Iterator, T2Iterator>>
      Zip<T1, T1Iterator, T2, T2Iterator> (
         this in Sequence<T1, T1Iterator> sequence1,
         Sequence<T2, T2Iterator> sequence2
      )
   where T1Iterator: IIterator<T1>
   where T2Iterator: IIterator<T2> {
      return sequence1.Zip<T1, T1Iterator, T2, T2Iterator, (T1, T2), TupleItemZipper<T1, T2>>(sequence2, new TupleItemZipper<T1, T2>());
   }
}
