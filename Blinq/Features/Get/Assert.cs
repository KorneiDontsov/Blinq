#define DEBUG

namespace Blinq;

public static partial class Get {
   [Conditional("DEBUG")]
   public static void Assert ([DoesNotReturnIf(false)] bool condition, [CallerArgumentExpression(nameof(condition))] string? message = null) {
      Debug.Assert(condition, message);
   }
}
