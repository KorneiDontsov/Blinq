namespace Blinq;

public readonly struct ListCollector<T>: ICollector<T, List<T>> {
   readonly List<T> List = new();

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ListCollector () { }

   public int Capacity { [MethodImpl(MethodImplOptions.AggressiveInlining)] set => List.Capacity = value; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Finalize (ref List<T> builder) { }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (T item) {
      List.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public List<T> Build () {
      return List;
   }
}

public static partial class Collectors {
   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Contract<ICollector<T, List<T>>, ListCollector<T>> List<T> (this CollectorProvider<T> collectorProvider) {
      return new ListCollector<T>();
   }
}
