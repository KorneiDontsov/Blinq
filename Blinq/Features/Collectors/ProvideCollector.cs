namespace Blinq;

[Pure]
public delegate Contract<ICollector<T, TCollection>, TCollector> ProvideCollector<T, TCollection, TCollector> (
   CollectorProvider<T> collectorProvider = default
) where TCollector: ICollector<T, TCollection>;
