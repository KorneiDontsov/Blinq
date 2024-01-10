using System;

namespace Blinq.CodeGen;

[Flags]
enum TypeModifiers: byte {
   None = 0,
   Static = 1 << 0,
   Unsafe = 1 << 1,
   Partial = 1 << 2,
   Struct = 1 << 3,
   Record = 1 << 4,
}
