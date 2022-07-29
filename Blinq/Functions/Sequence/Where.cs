using System.Runtime.CompilerServices;

namespace Blinq;

struct WhereFoldFunc<T, TAccumulator, TInnerFoldFunc>: IFoldFunc<T, TAccumulator> where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   readonly Func<T, bool> Predicate;
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereFoldFunc (Func<T, bool> predicate, TInnerFoldFunc innerFoldFunc) {
      Predicate = predicate;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return Predicate(item) && InnerFoldFunc.Invoke(item, ref accumulator);
   }
}

public struct WhereIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly Func<T, bool> Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereIterator (TIterator iterator, Func<T, bool> predicate) {
      Iterator = iterator;
      Predicate = predicate;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return Iterator.Fold(seed, new WhereFoldFunc<T, TAccumulator, TFoldFunc>(Predicate, func));
   }
}

public static partial class Sequence {
   /// <summary>Filters a sequence of values based on a predicate.</summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <returns>A sequence that contains elements from the input sequence that satisfy the condition.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, TIterator>> Where<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, bool> predicate)
   where TIterator: IIterator<T> {
      return new WhereIterator<T, TIterator>(sequence.Iterator, predicate);
   }
}
