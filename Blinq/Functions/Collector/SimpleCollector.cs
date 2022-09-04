using System.Collections.Generic;

namespace Blinq;

public readonly struct SimpleCollector<T, TCollection>: ICollector<T, TCollection, TCollection> where TCollection: ICollection<T>, new() {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TCollection CreateBuilder () {
      return new TCollection();
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Add (ref TCollection builder, T item) {
      builder.Add(item);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public TCollection Build (in TCollection builder) {
      return builder;
   }
}
