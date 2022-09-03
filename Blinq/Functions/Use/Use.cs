namespace Blinq;

public readonly struct Use<T, TContract> {
   public readonly TContract Contract;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Use (TContract contract) {
      Contract = contract;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public void Deconstruct (out TContract contract) {
      contract = Contract;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Use<T, TContract> (TContract contract) {
      return new Use<T, TContract>(contract);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TContract (Use<T, TContract> use) {
      return use.Contract;
   }
}

public readonly partial struct Use<T> {
   public static Use<T> Itself { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => default; }
}
