using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Blinq.Benchmarks;

[StructLayout(LayoutKind.Sequential)]
[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
struct Int64X8 {
   public long Number0;
   public long Number1;
   public long Number2;
   public long Number3;
   public long Number4;
   public long Number5;
   public long Number6;
   public long Number7;
}
