namespace Blinq;

public readonly struct ListCollector<T>: ICollector<T, List<T>, List<T>> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public List<T> CreateBuilder (int expectedCapacity = 0) {
      return new(expectedCapacity);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (ref List<T> list, T item) {
      list.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public List<T> Build (ref List<T> list) {
      return list;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Finalize (ref List<T> builder) { }
}

public static partial class Collector {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<ICollector<T, List<T>, List<T>>, ListCollector<T>> List<T> (this CollectorProvider<T> _) {
      return new ListCollector<T>();
   }
}
