using System;

namespace Blinq.CodeGen;

[Flags]
enum MethodModifiers: byte {
   None = 0,
   Static = 1 << 0,
   Extension = 1 << 1,
}
