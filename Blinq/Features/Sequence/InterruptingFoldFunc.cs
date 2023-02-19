namespace Blinq;

readonly struct InterruptingFold<T, TAccumulator, TInnerFold>: IFold<T, (TAccumulator Accumulator, bool Interrupted)>
where TInnerFold: IFold<T, TAccumulator> {
   readonly TInnerFold InnerFold;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public InterruptingFold (TInnerFold innerFold) {
      InnerFold = innerFold;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator Accumulator, bool Interrupted) state) {
      return state.Interrupted = InnerFold.Invoke(item, ref state.Accumulator);
   }
}
