namespace Blinq;

static partial class Utils {
   [MethodImpl(MethodImplOptions.NoInlining)]
   [DoesNotReturn]
   public static void Throw<TException> () where TException: Exception, new() {
      throw new TException();
   }
}
