using System.Numerics;
using System.Runtime.CompilerServices;

namespace Blinq;

public static partial class Iterator {
   public interface ICountImpl<TCount> {
      TCount countZero { get; }
      void Increment (ref TCount count);
   }

   public readonly struct DefaultCountImpl<TCount>: ICountImpl<TCount>
   where TCount: INumberBase<TCount> {
      public static Pin<ICountImpl<TCount>, DefaultCountImpl<TCount>> New () {
         return default;
      }

      public TCount countZero => TCount.Zero;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Increment (ref TCount count) {
         checked {
            ++count;
         }
      }
   }
}

readonly struct CountVisitor<T, TCount, TImpl>: IIteratorVisitor<T, TCount>
where TImpl: Iterator.ICountImpl<TCount> {
   public required TImpl impl { get; init; }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Visit (ref TCount state, in T item) {
      impl.Increment(ref state);
      return false;
   }
}

public static partial class Iterator {
   public static TCount Count<T, TIterator, TCount, TImpl> (
      this Pin<IIterator<T>, TIterator> iterator,
      Pin<ICountImpl<TCount>, TImpl> impl
   )
   where TIterator: IIterator<T>
   where TImpl: ICountImpl<TCount> {
      var count = impl.value.countZero;
      iterator.value.Accept(ref count, new CountVisitor<T, TCount, TImpl> { impl = impl });
      return count;
   }

   public static TCount Count<T, TIterator, TCount> (
      this Pin<IIterator<T>, TIterator> iterator,
      TypePin<TCount> tCount = default
   )
   where TIterator: IIterator<T>
   where TCount: INumberBase<TCount> {
      _ = tCount;
      return Count(iterator, DefaultCountImpl<TCount>.New());
   }

   public static int Count<T, TIterator> (this Pin<IIterator<T>, TIterator> iterator)
   where TIterator: IIterator<T> {
      return Count(iterator, Pin<int>.type);
   }

   public static long LongCount<T, TIterator> (this Pin<IIterator<T>, TIterator> iterator)
   where TIterator: IIterator<T> {
      return Count(iterator, Pin<long>.type);
   }
}
