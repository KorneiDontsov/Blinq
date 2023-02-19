namespace Blinq.Collections;

interface ILightEnumerator<T> {
   T Current { get; }
   bool MoveNext ();
}
