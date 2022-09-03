using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct EqualerProvider<T> { }

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public delegate TEqualer ProvideEqualer<T, TEqualer> (EqualerProvider<T> equalerProvider);

public static partial class Equaler {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TEqualer Invoke<T, TEqualer> (this ProvideEqualer<T, TEqualer> provideEqualer) where TEqualer: IEqualityComparer<T> {
      return provideEqualer(new EqualerProvider<T>());
   }
}
