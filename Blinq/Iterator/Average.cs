using System.Numerics;
using System.Runtime.CompilerServices;

namespace Blinq;

readonly struct AverageVisitor<T, TSum, TCount, TImpl>
   : IIteratorVisitor<T, (TSum sum, TCount count)>
where TImpl: Iterator.ISumImpl<T, TSum>, Iterator.ICountImpl<TCount> {
   public required TImpl impl { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Visit (ref (TSum sum, TCount count) state, in T item) {
      impl.Add(ref state.sum, in item);
      impl.Increment(ref state.count);
      return false;
   }
}

public static partial class Iterator {
   public interface IAverageImpl<T, TSum, TCount, TAverage>
      : ISumImpl<T, TSum>, ICountImpl<TCount> {
      Option<TAverage> Divide (in TSum left, in TCount right);
   }

   public readonly struct DefaultAverageImpl<T>: IAverageImpl<T, T, T, T>
   where T: INumberBase<T> {
      public static Pin<IAverageImpl<T, T, T, T>, DefaultAverageImpl<T>> New () {
         return default;
      }

      public T zero => T.Zero;
      public T countZero => T.Zero;

      public void Add (ref T left, in T right) {
         checked {
            left += right;
         }
      }

      public void Increment (ref T count) {
         checked {
            ++count;
         }
      }

      public Option<T> Divide (in T left, in T right) {
         if (T.IsZero(right)) return Option.none;
         return left / right;
      }
   }

   public static Option<TResult> Average<T, TIterator, TSum, TCount, TResult, TImpl> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<IAverageImpl<T, TSum, TCount, TResult>, TImpl> impl
   )
   where T: IAdditionOperators<T, T, T>
   where TIterator: IIterator<T>
   where TCount: IIncrementOperators<TCount>
   where TImpl: IAverageImpl<T, TSum, TCount, TResult> {
      var state = (sum: impl.value.zero, count: impl.value.countZero);
      iterator.value.Accept(ref state, new AverageVisitor<T, TSum, TCount, TImpl> { impl = impl });
      return impl.value.Divide(in state.sum, in state.count);
   }

   public static Option<T> Average<T, TIterator> (this Pin<IIterator<T>, TIterator> iterator)
   where T: INumberBase<T>
   where TIterator: IIterator<T> {
      return Average(iterator, DefaultAverageImpl<T>.New());
   }
}
