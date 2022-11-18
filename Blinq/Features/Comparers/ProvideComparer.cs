namespace Blinq;

[Pure]
public delegate TComparer ProvideComparer<T, TComparer> (ComparerProvider<T> comparerProvider = default);
