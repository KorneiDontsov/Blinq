using System.Runtime.InteropServices;

namespace Blinq.Collections;

[StructLayout(LayoutKind.Sequential)]
struct TableCell<TEntry> where TEntry: notnull {
   public int HashCode;
   public TableIndex Next;
   public TableIndex Previous;
   public TEntry Entry;
}
