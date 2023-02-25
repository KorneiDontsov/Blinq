using Blinq.Functors;

namespace Blinq;

readonly struct ZipFold<TIn1, TAccumulator, TIn2, TOut, TZipper, TIn2Iterator, TInnerFold>:
   IFold<TIn1, (TAccumulator accumulator, TIn2Iterator iterator2)>
where TIn2Iterator: IIterator<TIn2>
where TZipper: IZipper<TIn1, TIn2, TOut>
where TInnerFold: IFold<TOut, TAccumulator> {
   readonly TZipper Zipper;
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ZipFold (TZipper zipper, TInnerFold innerFold) {
      Zipper = zipper;
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TIn1 item1, ref (TAccumulator accumulator, TIn2Iterator iterator2) state) {
      return state.iterator2.TryPop(out var item2)
         && InnerFold.Invoke(Zipper.Invoke(item1, item2), ref state.accumulator);
   }
}

public struct ZipIterator<TOut, TIn1, TIn2, TZipper, TIn1Iterator, TIn2Iterator>: IIterator<TOut>
where TIn1Iterator: IIterator<TIn1>
where TIn2Iterator: IIterator<TIn2>
where TZipper: IZipper<TIn1, TIn2, TOut> {
   TIn1Iterator Iterator1;
   TIn2Iterator Iterator2;
   readonly TZipper Zipper;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ZipIterator (TIn1Iterator iterator1, TIn2Iterator iterator2, TZipper zipper) {
      Iterator1 = iterator1;
      Iterator2 = iterator2;
      Zipper = zipper;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      if (Iterator1.TryPop(out var item1) && Iterator2.TryPop(out var item2)) {
         item = Zipper.Invoke(item1, item2);
         return true;
      } else {
         item = default;
         return false;
      }
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator accumulator, TFold fold) where TFold: IFold<TOut, TAccumulator> {
      (accumulator, Iterator2) =
         Iterator1.Fold((seed: accumulator, Iterator2), new ZipFold<TIn1, TAccumulator, TIn2, TOut, TZipper, TIn2Iterator, TFold>(Zipper, fold));
      return accumulator;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      if (Iterator1.TryGetCount(out count) && Iterator2.TryGetCount(out var count2)) {
         if (count > count2) count = count2;
         return true;
      } else {
         return false;
      }
   }
}

public static partial class Iterator {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TResult>, ZipIterator<TResult, T1, T2, TZipper, T1Iterator, T2Iterator>>
      Zip<T1, T1Iterator, T2, T2Iterator, TResult, TZipper> (
         this in Contract<IIterator<T1>, T1Iterator> iterator1,
         Contract<IIterator<T2>, T2Iterator> iterator2,
         Contract<IZipper<T1, T2, TResult>, TZipper> zipper
      )
   where T1Iterator: IIterator<T1>
   where T2Iterator: IIterator<T2>
   where TZipper: IZipper<T1, T2, TResult> {
      return new ZipIterator<TResult, T1, T2, TZipper, T1Iterator, T2Iterator>(iterator1, iterator2, zipper);
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<TResult>, ZipIterator<TResult, T1, T2, FuncZipper<T1, T2, TResult>, T1Iterator, T2Iterator>>
      Zip<T1, T1Iterator, T2, T2Iterator, TResult> (
         this in Contract<IIterator<T1>, T1Iterator> iterator1,
         Contract<IIterator<T2>, T2Iterator> iterator2,
         Func<T1, T2, TResult> zipper
      )
   where T1Iterator: IIterator<T1>
   where T2Iterator: IIterator<T2> {
      return iterator1.Zip(iterator2, Get<IZipper<T1, T2, TResult>>.AsContract(new FuncZipper<T1, T2, TResult>(zipper)));
   }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<(T1, T2)>, ZipIterator<(T1, T2), T1, T2, TupleZipper<T1, T2>, T1Iterator, T2Iterator>>
      Zip<T1, T1Iterator, T2, T2Iterator> (
         this in Contract<IIterator<T1>, T1Iterator> iterator1,
         Contract<IIterator<T2>, T2Iterator> iterator2
      )
   where T1Iterator: IIterator<T1>
   where T2Iterator: IIterator<T2> {
      return iterator1.Zip(iterator2, Get<IZipper<T1, T2, (T1, T2)>>.AsContract(new TupleZipper<T1, T2>()));
   }
}
