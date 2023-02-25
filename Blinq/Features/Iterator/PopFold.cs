namespace Blinq;

readonly struct PopFold<T>: IFold<T, Option<T>> {
   public bool Invoke (T item, ref Option<T> accumulator) {
      accumulator = item;
      return true;
   }
}
