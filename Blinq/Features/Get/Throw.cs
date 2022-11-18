namespace Blinq;

public static partial class Get {
   [DoesNotReturn] [MethodImpl(MethodImplOptions.NoInlining)]
   public static void Throw () {
      throw new ApplicationException();
   }

   [DoesNotReturn] [MethodImpl(MethodImplOptions.NoInlining)]
   public static void Throw (string message) {
      throw new ApplicationException(message);
   }

   [DoesNotReturn] [MethodImpl(MethodImplOptions.NoInlining)]
   public static void Throw<TException> () where TException: Exception, new() {
      throw new TException();
   }
}
