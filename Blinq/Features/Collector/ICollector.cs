namespace Blinq;

public interface ICollector<T, TCollection> {
   void EnsureCapacity (int minCapacity);
   void Add (T item);
   TCollection Build ();
}
