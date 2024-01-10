namespace Blinq.CodeGen;

sealed class CodeGenSymbols {
   public required string openGenericBracket { get; init; }
   public required string closedGenericBracket { get; init; }
   public required string listSeparator { get; init; }

   public static CodeGenSymbols @default { get; } =
      new() {
         openGenericBracket = "<",
         closedGenericBracket = ">",
         listSeparator = ", ",
      };

   public static CodeGenSymbols titleize { get; } =
      new() {
         openGenericBracket = "[",
         closedGenericBracket = "]",
         listSeparator = ",",
      };
}
