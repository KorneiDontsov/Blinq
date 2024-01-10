namespace Blinq;

public struct Pin<TAbstraction, TImplementation> where TImplementation: TAbstraction {
   public required TImplementation value;

   public static implicit operator Pin<TAbstraction, TImplementation> (TImplementation value) {
      return new() { value = value };
   }

   public static implicit operator TImplementation (Pin<TAbstraction, TImplementation> pin) {
      return pin.value;
   }
}

public static class Pin<T> {
   public static TypePin<T> type => default;
}
