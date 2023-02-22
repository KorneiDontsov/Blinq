namespace Blinq;

public interface ICollector<T, TCollection> {
   int Capacity { set; }
   void Add (T item);
   TCollection Build ();
}
