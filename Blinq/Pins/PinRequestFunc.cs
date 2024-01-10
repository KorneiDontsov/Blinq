namespace Blinq;

public delegate Pin<TAbstraction, TImplementation> PinRequestFunc<TAbstraction, TImplementation> (
   PinRequest<TAbstraction> request = default
) where TImplementation: TAbstraction;
