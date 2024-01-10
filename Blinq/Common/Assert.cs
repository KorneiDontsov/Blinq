using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Blinq;

public static class Assert {
   /// <exception cref="AssertException"><paramref name="condition" /> is false.</exception>
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void That (
      [DoesNotReturnIf(false)] bool condition,
      [CallerArgumentExpression(nameof(condition))] string? message = null
   ) {
      [MethodImpl(MethodImplOptions.NoInlining)]
      [DoesNotReturn]
      static void Throw (string? message) {
         throw new AssertException(message);
      }

      if (!condition) Throw(message);
   }

   /// <exception cref="AssertException"><paramref name="condition" /> is false.</exception>
   [Conditional("DEBUG")]
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static void Debug (
      [DoesNotReturnIf(false)] bool condition,
      [CallerArgumentExpression(nameof(condition))] string? message = null
   ) {
      That(condition, message);
   }
}
