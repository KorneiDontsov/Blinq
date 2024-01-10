using System;

namespace Blinq;

public sealed class AssertException: Exception {
   public AssertException (string? message): base(message) { }
}
