namespace Blinq;

static class Throw {
   [MethodImpl(MethodImplOptions.NoInlining)]
   public static void InvalidOperationException_IteratorIsNotAllowedToBeEnumeratedTwice () {
      throw new InvalidOperationException(Resources.InvalidOperationException_IteratorIsNotAllowedToBeEnumeratedTwice);
   }

   [MethodImpl(MethodImplOptions.NoInlining)]
   public static void InvalidOperationException_ConcurrentOperationsAreNotSupported () {
      throw new InvalidOperationException(Resources.InvalidOperationException_ConcurrentOperationsAreNotSupported);
   }
}
