namespace Blinq;

public readonly partial struct Use<T> {
   public static Use<T> Here { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default; }
}

public readonly struct Use<TContract, T> where T: TContract {
   public readonly T Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Use (T value) {
      Value = value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Use<TContract, T> (T value) {
      return new Use<TContract, T>(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator T (Use<TContract, T> use) {
      return use.Value;
   }
}
