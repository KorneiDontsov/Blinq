namespace Blinq;

readonly struct FlattenInFoldFunc<TOut, TAccumulator, TOutAccumulator, TOutIterator>:
   IFoldFunc<Sequence<TOut, TOutIterator>, (TAccumulator Accumulator, TOutIterator OutIterator, bool Interrupted)>
where TOutIterator: IIterator<TOut>
where TOutAccumulator: IFoldFunc<TOut, TAccumulator> {
   readonly InterruptingFoldFunc<TOut, TAccumulator, TOutAccumulator> OutFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FlattenInFoldFunc (InterruptingFoldFunc<TOut, TAccumulator, TOutAccumulator> outFoldFunc) {
      OutFoldFunc = outFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (Sequence<TOut, TOutIterator> item, ref (TAccumulator Accumulator, TOutIterator OutIterator, bool Interrupted) state) {
      state.OutIterator = item.Iterator;
      (state.Accumulator, state.Interrupted) = state.OutIterator.Fold((state.Accumulator, Interrupted: false), OutFoldFunc);
      return state.Interrupted;
   }
}

public struct FlattenIterator<TOut, TOutIterator, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<Sequence<TOut, TOutIterator>>
where TOutIterator: IIterator<TOut> {
   TInIterator InIterator;
   TOutIterator OutIterator;
   bool Interrupted;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FlattenIterator (TInIterator inIterator) {
      InIterator = inIterator;
      OutIterator = default!;
      Interrupted = false;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<TOut, TAccumulator> {
      var outFoldFunc = new InterruptingFoldFunc<TOut, TAccumulator, TFoldFunc>(func);
      if (Interrupted) {
         (seed, Interrupted) = OutIterator.Fold((seed, Interrupted: false), outFoldFunc);
      }

      if (!Interrupted) {
         (seed, OutIterator, Interrupted) =
            InIterator.Fold(
               (seed, default(TOutIterator)!, Interrupted: false),
               new FlattenInFoldFunc<TOut, TAccumulator, TFoldFunc, TOutIterator>(outFoldFunc)
            );
      }

      return seed;
   }
}

public static partial class Sequence {
   /// <summary>Flattens a sequence of the sequences into one sequence.</summary>
   /// <returns>A sequence whose elements are the elements of the input sequences.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, FlattenIterator<T, TInnerIterator, TIterator>> Flatten<T, TIterator, TInnerIterator> (
      this in Sequence<Sequence<T, TInnerIterator>, TIterator> sequence
   )
   where TIterator: IIterator<Sequence<T, TInnerIterator>>
   where TInnerIterator: IIterator<T> {
      return new FlattenIterator<T, TInnerIterator, TIterator>(sequence.Iterator);
   }
}
