namespace Blinq;

readonly struct FlattenFold<TOut, TAccumulator, TOutAccumulator, TOutIterator>:
   IFold<Contract<IIterator<TOut>, TOutIterator>, (TAccumulator Accumulator, TOutIterator OutIterator, bool Interrupted)>
where TOutIterator: IIterator<TOut>
where TOutAccumulator: IFold<TOut, TAccumulator> {
   readonly InterruptingFold<TOut, TAccumulator, TOutAccumulator> OutFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FlattenFold (InterruptingFold<TOut, TAccumulator, TOutAccumulator> outFold) {
      OutFold = outFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (
      Contract<IIterator<TOut>, TOutIterator> item,
      ref (TAccumulator Accumulator, TOutIterator OutIterator, bool Interrupted) state
   ) {
      state.OutIterator = item;
      (state.Accumulator, state.Interrupted) = state.OutIterator.Fold((state.Accumulator, Interrupted: false), OutFold);
      return state.Interrupted;
   }
}

public struct FlattenIterator<TOut, TOutIterator, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<Contract<IIterator<TOut>, TOutIterator>>
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
   public bool TryPop ([MaybeNullWhen(false)] out TOut item) {
      if (Interrupted && OutIterator.TryPop(out item)) return true;

      var popFold = new PopFold<TOut>();
      var interruptingPopFold = new InterruptingFold<TOut, Option<TOut>, PopFold<TOut>>(popFold);
      var flattenFold = new FlattenFold<TOut, Option<TOut>, PopFold<TOut>, TOutIterator>(interruptingPopFold);
      (var result, OutIterator, Interrupted) = InIterator.Fold((Option<TOut>.None, default(TOutIterator)!, Interrupted: false), flattenFold);
      return result.Is(out item);
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFold> (TAccumulator seed, TFold fold) where TFold: IFold<TOut, TAccumulator> {
      var interruptingFold = new InterruptingFold<TOut, TAccumulator, TFold>(fold);
      if (Interrupted) {
         (seed, Interrupted) = OutIterator.Fold((seed, Interrupted: false), interruptingFold);
      }

      if (!Interrupted) {
         var flattenFold = new FlattenFold<TOut, TAccumulator, TFold, TOutIterator>(interruptingFold);
         (seed, OutIterator, Interrupted) = InIterator.Fold((seed, default(TOutIterator)!, Interrupted: false), flattenFold);
      }

      return seed;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryGetCount (out int count) {
      count = default;
      return false;
   }
}

public static partial class Iterator {
   /// <summary>Flattens a sequence of the sequences into one sequence.</summary>
   /// <returns>A sequence whose elements are the elements of the input sequences.</returns>
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<IIterator<T>, FlattenIterator<T, TInnerIterator, TIterator>> Flatten<T, TIterator, TInnerIterator> (
      this in Contract<IIterator<Contract<IIterator<T>, TInnerIterator>>, TIterator> iterator
   )
   where TIterator: IIterator<Contract<IIterator<T>, TInnerIterator>>
   where TInnerIterator: IIterator<T> {
      return new FlattenIterator<T, TInnerIterator, TIterator>(iterator);
   }
}
