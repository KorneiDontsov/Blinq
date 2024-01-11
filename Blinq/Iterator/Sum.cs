using System.Numerics;
using System.Runtime.CompilerServices;

namespace Blinq;

public static partial class Iterator {
   public interface ISumImpl<T, TSum> {
      TSum zero { get; }
      void Add (ref TSum sum, in T value);
   }

   public readonly struct DefaultSumImpl<T>: ISumImpl<T, T>
   where T: INumberBase<T> {
      public static Pin<ISumImpl<T, T>, DefaultSumImpl<T>> New () {
         return default;
      }

      public T zero => T.Zero;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Add (ref T left, in T right) {
         checked {
            left += right;
         }
      }
   }
}

readonly struct SumVisitor<T, TSum, TImpl>: IIteratorVisitor<T, TSum>
where TImpl: Iterator.ISumImpl<T, TSum> {
   public required TImpl impl { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Visit (ref TSum state, in T item) {
      this.impl.Add(ref state, in item);
      return false;
   }
}

public static partial class Iterator {
   public static TSum Sum<T, TIterator, TSum, TImpl> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<ISumImpl<T, TSum>, TImpl> impl
   )
   where T: IAdditionOperators<T, T, T>
   where TIterator: IIterator<T>
   where TImpl: ISumImpl<T, TSum> {
      var sum = impl.value.zero;
      iterator.value.Accept(ref sum, new SumVisitor<T, TSum, TImpl> { impl = impl });
      return sum;
   }

   public static T Sum<T, TIterator> (this Pin<IIterator<T>, TIterator> iterator)
   where T: INumberBase<T>
   where TIterator: IIterator<T> {
      return Sum(iterator, DefaultSumImpl<T>.New());
   }
}
