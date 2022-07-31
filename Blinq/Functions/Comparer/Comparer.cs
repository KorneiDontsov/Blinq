using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public readonly struct ComparerProvider<T> { }

public static partial class Comparer {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static TComparer Invoke<T, TComparer> (this Func<ComparerProvider<T>, TComparer> comparerProvider) where TComparer: IComparer<T> {
      return comparerProvider.Invoke(new ComparerProvider<T>());
   }
}
