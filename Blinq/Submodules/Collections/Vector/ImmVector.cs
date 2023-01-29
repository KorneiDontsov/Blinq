namespace Blinq.Collections;

public sealed class ImmVector<T>: Vector<T> {
   public static ImmVector<T> Empty { get; } = new(new ValueVector<T>());

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   ImmVector (ValueVector<T> value): base(value) { }

   [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal static ImmVector<T> Create (ValueVector<T> value) {
      return value.Count > 0 ? new(value) : Empty;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public override ImmVector<T> ToImmutable () {
      return this;
   }
}
