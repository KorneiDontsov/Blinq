using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

public struct ArrayIterator<T>: IIterator<T> {
   public required T[] array { get; init; }
   int currentIndex = -1;

   public ArrayIterator () { }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      var nextIndex = currentIndex + 1;
      if (nextIndex < array.Length) {
         item = array[nextIndex];
         currentIndex = nextIndex;
         return true;
      }

      Advanced.SkipInit(out item);
      return false;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Accept<TAccumulate, TVisitor> (ref TAccumulate state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<T, TAccumulate> {
      int index;
      for (index = currentIndex + 1; index < array.Length; ++index) {
         if (visitor.Visit(ref state, in array[index])) {
            break;
         }
      }

      currentIndex = index;
   }
}

public static partial class ArrayExtensions {
   public static Pin<IIterator<T>, ArrayIterator<T>> Iterate<T> (this T[] array) {
      return new() { value = new() { array = array } };
   }
}
