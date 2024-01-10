using System.Runtime.CompilerServices;

namespace Blinq;

static class Advanced {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void SkipInit<T> (out T value) {
      if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
         value = default!;
      } else {
         Unsafe.SkipInit(out value);
      }
   }
}
