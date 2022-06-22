namespace Blinq;

public struct EmptyIterator<T>: IIterator<T> {
   public T Current => default!;

   public bool MoveNext () {
      return false;
   }
}

public static partial class Sequence {
   /// <summary>Returns empty sequence of a specified type.</summary>
   /// <typeparam name="T">The type of elements of a sequence.</typeparam>
   /// <returns>Empty sequence.</returns>
   public static Sequence<T, EmptyIterator<T>> Empty<T> () {
      return default;
   }
}
