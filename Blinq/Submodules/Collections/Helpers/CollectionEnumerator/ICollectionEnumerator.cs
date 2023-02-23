namespace Blinq.Collections;

interface ICollectionEnumerator<T> {
   T Current { get; }
   bool MoveNext ();
}
