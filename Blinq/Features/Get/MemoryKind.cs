namespace Blinq;

public enum TypeMemoryKind: byte {
   Reference = 1,
   Unmanaged,
   Complex,
}

public static partial class Get<T> {
   [Pure] public static TypeMemoryKind MemoryKind {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get {
         if (default(T) is null) {
            return TypeMemoryKind.Reference;
         } else if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
            return TypeMemoryKind.Complex;
         } else {
            return TypeMemoryKind.Unmanaged;
         }
      }
   }
}
