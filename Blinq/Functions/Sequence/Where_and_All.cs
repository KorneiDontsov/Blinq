using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IItemPredicate<T> {
   bool Invoke (T item);
}

public readonly struct FuncItemPredicate<T>: IItemPredicate<T> {
   readonly Func<T, bool> Func;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public FuncItemPredicate (Func<T, bool> func) {
      Func = func;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item) {
      return Func(item);
   }
}

struct WhereFoldFunc<T, TAccumulator, TPredicate, TInnerFoldFunc>: IFoldFunc<T, TAccumulator>
where TPredicate: IItemPredicate<T>
where TInnerFoldFunc: IFoldFunc<T, TAccumulator> {
   TPredicate Predicate;
   TInnerFoldFunc InnerFoldFunc;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereFoldFunc (TPredicate predicate, TInnerFoldFunc innerFoldFunc) {
      Predicate = predicate;
      InnerFoldFunc = innerFoldFunc;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref TAccumulator accumulator) {
      return Predicate.Invoke(item) && InnerFoldFunc.Invoke(item, ref accumulator);
   }
}

struct AllFoldFunc<T, TPredicate>: IFoldFunc<T, bool> where TPredicate: IItemPredicate<T> {
   TPredicate Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public AllFoldFunc (TPredicate predicate) {
      Predicate = predicate;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool Invoke (T item, ref bool accumulator) {
      if (Predicate.Invoke(item)) {
         return false;
      } else {
         accumulator = false;
         return true;
      }
   }
}

public struct WhereIterator<T, TPredicate, TIterator>: IIterator<T>
where TPredicate: IItemPredicate<T>
where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly TPredicate Predicate;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public WhereIterator (TIterator iterator, TPredicate predicate) {
      Iterator = iterator;
      Predicate = predicate;
   }

   /// <inheritdoc />
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TAccumulator Fold<TAccumulator, TFoldFunc> (TAccumulator seed, TFoldFunc func) where TFoldFunc: IFoldFunc<T, TAccumulator> {
      return Iterator.Fold(seed, new WhereFoldFunc<T, TAccumulator, TPredicate, TFoldFunc>(Predicate, func));
   }
}

public static partial class Sequence {
   /// <summary>Filters a sequence of values based on a predicate.</summary>
   /// <param name="predicate">A function to test each element for a condition.</param>
   /// <returns>A sequence that contains elements from the input <paramref name="sequence" /> that satisfy the condition.</returns>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Sequence<T, WhereIterator<T, FuncItemPredicate<T>, TIterator>> Where<T, TIterator> (
      this in Sequence<T, TIterator> sequence,
      Func<T, bool> predicate
   )
   where TIterator: IIterator<T> {
      return new WhereIterator<T, FuncItemPredicate<T>, TIterator>(sequence.Iterator, new FuncItemPredicate<T>(predicate));
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool All<T, TIterator> (this in Sequence<T, TIterator> sequence, Func<T, bool> predicate) where TIterator: IIterator<T> {
      return sequence.Iterator.Fold(true, new AllFoldFunc<T, FuncItemPredicate<T>>(new FuncItemPredicate<T>(predicate)));
   }
}
