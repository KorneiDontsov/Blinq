using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct ComparerProvider<T> { }

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public delegate TComparer ProvideComparer<T, TComparer> (ComparerProvider<T> comparerProvider);

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TComparer Invoke<T, TComparer> (this ProvideComparer<T, TComparer> provideComparer) where TComparer: IComparer<T> {
      return provideComparer.Invoke(new ComparerProvider<T>());
   }
}
