using System.Runtime.CompilerServices;

namespace Blinq;

struct FlattenOutAccumulator<TOut, TAccumulated, TNextAccumulator>: IAccumulator<TOut, (TAccumulated Accumulated, bool Interrupted)>
where TNextAccumulator: IAccumulator<TOut, TAccumulated> {
   TNextAccumulator NextAccumulator;

   public FlattenOutAccumulator (TNextAccumulator nextAccumulator) {
      NextAccumulator = nextAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (TOut item, ref (TAccumulated Accumulated, bool Interrupted) state) {
      return state.Interrupted = NextAccumulator.Invoke(item, ref state.Accumulated);
   }
}

readonly struct FlattenInAccumulator<TOut, TAccumulated, TOutAccumulator, TOutIterator>:
   IAccumulator<Sequence<TOut, TOutIterator>, (TAccumulated Accumulated, TOutIterator OutIterator, bool Interrupted)>
where TOutIterator: IIterator<TOut>
where TOutAccumulator: IAccumulator<TOut, TAccumulated> {
   readonly FlattenOutAccumulator<TOut, TAccumulated, TOutAccumulator> OutAccumulator;

   public FlattenInAccumulator (FlattenOutAccumulator<TOut, TAccumulated, TOutAccumulator> outAccumulator) {
      OutAccumulator = outAccumulator;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (Sequence<TOut, TOutIterator> item, ref (TAccumulated Accumulated, TOutIterator OutIterator, bool Interrupted) state) {
      state.OutIterator = item.Iterator;
      (state.Accumulated, state.Interrupted) = state.OutIterator.Accumulate(OutAccumulator, (state.Accumulated, Interrupted: false));
      return state.Interrupted;
   }
}

public struct FlattenIterator<TOut, TOutIterator, TInIterator>: IIterator<TOut>
where TInIterator: IIterator<Sequence<TOut, TOutIterator>>
where TOutIterator: IIterator<TOut> {
   TInIterator InIterator;
   TOutIterator OutIterator;
   bool Interrupted;

   public FlattenIterator (TInIterator inIterator) {
      InIterator = inIterator;
      OutIterator = default!;
      Interrupted = false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulated Accumulate<TAccumulated, TAccumulator> (TAccumulator accumulator, TAccumulated seed)
   where TAccumulator: IAccumulator<TOut, TAccumulated> {
      var outAccumulator = new FlattenOutAccumulator<TOut, TAccumulated, TAccumulator>(accumulator);
      if (Interrupted) {
         (seed, Interrupted) = OutIterator.Accumulate(outAccumulator, (seed, Interrupted: false));
      }

      if (!Interrupted) {
         (seed, OutIterator, Interrupted) =
            InIterator.Accumulate(
               new FlattenInAccumulator<TOut, TAccumulated, TAccumulator, TOutIterator>(),
               (seed, default(TOutIterator)!, Interrupted: false)
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
