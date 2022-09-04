using Blinq.Math;

namespace Blinq;

public readonly partial struct Use<T> {
   public static Use<IMath<T>, TMath> Math<TMath> (ProvideMath<T, TMath> provideMath) where TMath: IMath<T> {
      return provideMath.Invoke();
   }
}
