namespace System;

static class CharExtensions {
   public static bool IsAsciiDigit (this char c) {
      return c is >= '0' and <= '9';
   }
}
