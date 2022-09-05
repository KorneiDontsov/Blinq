namespace Blinq;

public readonly partial struct Use<T> {
   public static Use<T> Here { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default; }
}

public readonly struct Use<TContract, TValue> where TValue: TContract {
   public readonly TValue Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Use (TValue value) {
      Value = value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Use<TContract, TValue> (TValue value) {
      return new Use<TContract, TValue>(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TValue (Use<TContract, TValue> use) {
      return use.Value;
   }
}
