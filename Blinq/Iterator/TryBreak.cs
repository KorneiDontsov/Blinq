namespace Blinq;

readonly struct TryBreakVisitor<T, TState, TImpl>
   : IIteratorVisitor<T, (TState coreState, bool hasBreak)>
where TImpl: IIteratorVisitor<T, TState> {
   public required TImpl impl { get; init; }

   public bool Visit (ref (TState coreState, bool hasBreak) state, in T item) {
      var result = impl.Visit(ref state.coreState, in item);
      state.hasBreak = result;
      return result;
   }
}

public static partial class Iterator {
   public static bool TryBreak<T, TIterator, TState, TVisitor> (
      ref TIterator iterator,
      ref TState state,
      TVisitor visitor,
      TypePin<T> t = default
   )
   where TIterator: IIterator<T>
   where TVisitor: IIteratorVisitor<T, TState> {
      _ = t;

      var extendedState = (coreState: state, hasBreak: false);
      iterator.Accept(
         ref extendedState,
         new TryBreakVisitor<T, TState, TVisitor> { impl = visitor }
      );
      state = extendedState.coreState;
      return extendedState.hasBreak;
   }
}
