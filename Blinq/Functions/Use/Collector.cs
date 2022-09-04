namespace Blinq;

public readonly partial struct Use<T> {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Use<ICollector<T, TCollection, TBuilder>, TCollector> Collector<TCollection, TBuilder, TCollector> (
      ProvideCollector<T, TCollection, TBuilder, TCollector> provideCollector
   ) where TCollector: ICollector<T, TCollection, TBuilder> {
      return provideCollector.Invoke();
   }
}
