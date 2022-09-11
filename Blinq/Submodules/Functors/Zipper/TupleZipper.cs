namespace Blinq.Functors;

public readonly struct TupleZipper<T1, T2>: IZipper<T1, T2, (T1, T2)> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public (T1, T2) Invoke (T1 item1, T2 item2) {
      return (item1, item2);
   }
}
