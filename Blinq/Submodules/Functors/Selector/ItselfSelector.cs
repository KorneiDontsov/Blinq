namespace Blinq.Functors;

public readonly struct ItselfSelector<T>: ISelector<T, T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public T Invoke (T item) {
      return item;
   }
}
