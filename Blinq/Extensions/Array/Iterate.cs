using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

public struct ArrayIterator<T>: IIterator<T> {
   public required T[] array { get; init; }
   int currentIndex = -1;

   public ArrayIterator () { }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool TryPop ([MaybeNullWhen(false)] out T item) {
      var nextIndex = this.currentIndex + 1;
      if ((uint)nextIndex >= (uint)this.array.Length) {
         Unsafe.SkipInit(out item);
         return false;
      }

      item = this.array[nextIndex];
      this.currentIndex = nextIndex;
      return true;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Accept<TAccumulate, TVisitor> (ref TAccumulate state, TVisitor visitor)
   where TVisitor: IIteratorVisitor<T, TAccumulate> {
      var index = this.currentIndex;
      while (true) {
         ++index;
         if ((uint)index >= (uint)this.array.Length) break;

         this.currentIndex = index;
         if (visitor.Visit(ref state, in this.array[index])) break;
      }
   }
}

public static partial class ArrayExtensions {
   public static Pin<IIterator<T>, ArrayIterator<T>> Iterate<T> (this T[] array) {
      return new() { value = new() { array = array } };
   }
}
