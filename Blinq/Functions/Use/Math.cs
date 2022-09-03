using Blinq.Math;

namespace Blinq.Functions.Use;

public readonly partial struct Use<T> {
   public static Use<T, TMath> Math<TMath> (ProvideMath<T, TMath> provideMath) where TMath: IMathContract<T> {
      return provideMath.Invoke();
   }
}
