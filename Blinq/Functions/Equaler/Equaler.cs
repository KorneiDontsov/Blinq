using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct EqualerProvider<T> { }

public static partial class Equaler {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TEqualer Invoke<T, TEqualer> (this Func<EqualerProvider<T>, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return provideEqualer(new EqualerProvider<T>());
   }
}
