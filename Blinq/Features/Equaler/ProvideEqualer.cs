namespace Blinq;

[Pure]
public delegate TEqualer ProvideEqualer<T, TEqualer> (EqualerProvider<T> equalerProvider = default);
