using System.Collections.Generic;

namespace Blinq;

public static partial class Sequence {
   /// <summary>Creates a sequence over <see cref="IIterable{T,TIterator}" />.</summary>
   public static Sequence<T, TIterator> Iterate<T, TIterator> (this IIterable<T, TIterator> iterable) where TIterator: IIterator<T> {
      var count = iterable switch {
         ICollection<T> collection => collection.Count,
         IReadOnlyCollection<T> collection => collection.Count,
         _ => Option<int>.None,
      };
      return new Sequence<T, TIterator>(iterable.CreateIterator(), count);
   }

   /// <summary>Creates a sequence over <see cref="IIterableCollection{T,TIterator}" />.</summary>
   public static Sequence<T, TIterator> Iterate<T, TIterator> (this IIterableCollection<T, TIterator> collection) where TIterator: IIterator<T> {
      var count = collection.Count;
      return new Sequence<T, TIterator>(collection.CreateIterator(), count);
   }
}
