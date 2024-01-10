using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen;

record struct CodeGenContext {
   public SyntaxIndent indent { get; init; } = SyntaxIndent.none;
   public CodeGenSymbols symbols { get; init; } = CodeGenSymbols.@default;
   public INamespaceSymbol? currentNamespace { get; init; }

   public CodeGenContext () { }
}
