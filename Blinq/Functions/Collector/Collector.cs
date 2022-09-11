namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
[ReadOnly(true)]
public interface ICollector<T, TCollection, TBuilder> {
   TBuilder CreateBuilder (int expectedCapacity = 0);
   void Add (ref TBuilder builder, T item);
   TCollection Build (ref TBuilder builder);
   void Finalize (ref TBuilder builder);
}

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct CollectorProvider<T> { }

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
