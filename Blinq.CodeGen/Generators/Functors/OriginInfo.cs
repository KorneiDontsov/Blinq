using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.Functors;

sealed record OriginInfo {
   public required MethodSignature signature { get; init; }
   public required ReadOnlyValueList<ITypeSymbol> functorTypeArgSymbols { get; init; }
   public required ITypeParameterSymbol functorTypeParameterSymbol { get; init; }
   public required IParameterSymbol functorParameterSymbol { get; init; }
}
