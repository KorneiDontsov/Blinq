namespace Blinq;

public readonly struct Contract<TAbstraction, TValue> where TValue: TAbstraction {
   public readonly TValue Value;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Contract (TValue value) {
      Value = value;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator Contract<TAbstraction, TValue> (TValue value) {
      return new Contract<TAbstraction, TValue>(value);
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static implicit operator TValue (Contract<TAbstraction, TValue> contract) {
      return contract.Value;
   }
}
