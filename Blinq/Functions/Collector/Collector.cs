using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface ICollector<T, TCollection, TBuilder> {
   TBuilder CreateBuilder ();
   void Add (ref TBuilder builder, T item);
   TCollection Build (in TBuilder builder);
}

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct CollectorProvider<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Use<ICollector<T, TCollection, TCollection>, SimpleCollector<T, TCollection>> Collection<TCollection> ()
   where TCollection: ICollection<T>, new() {
      return new SimpleCollector<T, TCollection>();
   }
}

public delegate Use<ICollector<T, TCollection, TBuilder>, TCollector> ProvideCollector<T, TCollection, TBuilder, TCollector> (
   CollectorProvider<T> collectorProvider
) where TCollector: ICollector<T, TCollection, TBuilder>;

public static partial class Collector {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<ICollector<T, TCollection, TBuilder>, TCollector> Invoke<T, TCollection, TBuilder, TCollector> (
      this ProvideCollector<T, TCollection, TBuilder, TCollector> provideCollector
   ) where TCollector: ICollector<T, TCollection, TBuilder> {
      return provideCollector(new CollectorProvider<T>());
   }
}
