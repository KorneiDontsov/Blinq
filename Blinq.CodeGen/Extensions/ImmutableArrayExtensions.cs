using System.Collections.Immutable;

namespace Blinq.CodeGen;

static class ImmutableArrayExtensions {
   public static ReadOnlyValueList<T> ToValueList<T> (this ImmutableArray<T> immutableArray) {
      return immutableArray;
   }
}
