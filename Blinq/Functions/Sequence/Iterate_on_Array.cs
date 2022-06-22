namespace Blinq;

/// <inheritdoc />
/// <summary>An array iterator.</summary>
public struct ArrayIterator<T>: IIterator<T> {
   readonly T[] Array;
   int Index;

   public ArrayIterator (T[] array) {
      Array = array;
      Index = 0;
   }

   /// <inheritdoc />
   public T Current => Array[Index++];

   /// <inheritdoc />
   public bool MoveNext () {
      return Index < Array.Length;
   }
}

public static partial class Sequence {
   /// <summary>Creates a sequence over <paramref name="array" />.</summary>
   public static Sequence<T, ArrayIterator<T>> Iterate<T> (this T[] array) {
      return new Sequence<T, ArrayIterator<T>>(new ArrayIterator<T>(array), array.Length);
   }
}
