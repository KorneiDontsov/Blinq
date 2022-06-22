using System.Diagnostics.CodeAnalysis;

namespace ResearchBenchmarks.FasterSequence;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IIterator<T> {
   T Current { get; }
   bool MoveNext ();
}

public struct Sequence<T, TIterator> where TIterator: IIterator<T> {
   public TIterator Iterator;
   public int Count;
}

public readonly struct RoSequence<T, TIterator> where TIterator: IIterator<T> {
   public readonly TIterator Iterator;
   public readonly int Count;

   public RoSequence (TIterator iterator, int count = 0) {
      Iterator = iterator;
      Count = count;
   }
}

public struct ArrayIterator<T>: IIterator<T> {
   readonly T[] Array;
   int Index;

   public ArrayIterator (T[] array) {
      Array = array;
      Index = 0;
   }

   public T Current => Array[Index++];

   public bool MoveNext () {
      return Index < Array.Length;
   }
}

public struct WhereIterator<T, TIterator>: IIterator<T> where TIterator: IIterator<T> {
   TIterator Iterator;
   readonly Func<T, bool> Predicate;

   public T Current { get; private set; }

   public WhereIterator (TIterator iterator, Func<T, bool> predicate) {
      Iterator = iterator;
      Predicate = predicate;
      Current = default!;
   }

   public bool MoveNext () {
      while (Iterator.MoveNext()) {
         Current = Iterator.Current;
         if (Predicate(Current)) return true;
      }

      return false;
   }
}

public struct SelectIterator<TOut, TIn, TInIterator>: IIterator<TOut> where TInIterator: IIterator<TIn> {
   TInIterator InIterator;
   readonly Func<TIn, TOut> Selector;

   public SelectIterator (TInIterator inIterator, Func<TIn, TOut> selector) {
      InIterator = inIterator;
      Selector = selector;
   }

   public TOut Current {
      get {
         var value = InIterator.Current;
         return Selector(value);
      }
   }

   public bool MoveNext () {
      return InIterator.MoveNext();
   }
}

static class Functions {
   public static Sequence<T, ArrayIterator<T>> Iterate<T> (this T[] array) {
      return new Sequence<T, ArrayIterator<T>> {
         Iterator = new ArrayIterator<T>(array),
         Count = array.Length,
      };
   }

   public static RoSequence<T, ArrayIterator<T>> RoIterate<T> (this T[] array) {
      return new RoSequence<T, ArrayIterator<T>>(
         new ArrayIterator<T>(array),
         array.Length
      );
   }

   public static Sequence<T, WhereIterator<T, TIterator>> Where<T, TIterator> (this Sequence<T, TIterator> sequence, Func<T, bool> predicate)
   where TIterator: IIterator<T> {
      return new Sequence<T, WhereIterator<T, TIterator>> {
         Iterator = new WhereIterator<T, TIterator>(sequence.Iterator, predicate),
      };
   }

   public static RoSequence<T, WhereIterator<T, TIterator>> RoWhere<T, TIterator> (this in RoSequence<T, TIterator> sequence, Func<T, bool> predicate)
   where TIterator: IIterator<T> {
      return new RoSequence<T, WhereIterator<T, TIterator>>(
         new WhereIterator<T, TIterator>(sequence.Iterator, predicate)
      );
   }

   public static Sequence<TResult, SelectIterator<TResult, T, TIterator>> Select<T, TIterator, TResult> (
      this Sequence<T, TIterator> sequence,
      Func<T, TResult> selector
   ) where TIterator: IIterator<T> {
      return new Sequence<TResult, SelectIterator<TResult, T, TIterator>> {
         Iterator = new SelectIterator<TResult, T, TIterator>(sequence.Iterator, selector),
         Count = sequence.Count,
      };
   }

   public static RoSequence<TResult, SelectIterator<TResult, T, TIterator>> RoSelect<T, TIterator, TResult> (
      this in RoSequence<T, TIterator> sequence,
      Func<T, TResult> selector
   ) where TIterator: IIterator<T> {
      return new RoSequence<TResult, SelectIterator<TResult, T, TIterator>>(
         new SelectIterator<TResult, T, TIterator>(sequence.Iterator, selector),
         sequence.Count
      );
   }
}

[SuppressMessage("ReSharper", "UseArrayEmptyMethod")]
public class FasterSequenceBenchmarks {
   readonly object[] Array = new object[0];

   [Benchmark]
   public Sequence<string?, SelectIterator<string?, object, WhereIterator<object, ArrayIterator<object>>>> MutableSequenceStruct () {
      return Array.Iterate().Where(static obj => obj.GetHashCode() == 0).Select(static obj => obj.ToString());
   }

   [Benchmark]
   public RoSequence<string?, SelectIterator<string?, object, WhereIterator<object, ArrayIterator<object>>>> ImmutableSequenceStruct () {
      return Array.RoIterate().RoWhere(static obj => obj.GetHashCode() == 0).RoSelect(static obj => obj.ToString());
   }
}
