namespace Blinq;

struct InterruptingFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, (TAccumulator Accumulator, bool Interrupted)>
where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public InterruptingFoldFunc (TInnerFoldFunc innerFoldFunc) {
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref (TAccumulator Accumulator, bool Interrupted) state) {
      return state.Interrupted = InnerFoldFunc.Invoke(item, ref state.Accumulator);
   }
}
