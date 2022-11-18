namespace Blinq;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct Type<T> { }

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct Type<TAbstraction, T> where T: TAbstraction { }
