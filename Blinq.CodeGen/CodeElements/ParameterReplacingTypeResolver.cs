using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

class ParameterReplacingTypeResolver: TypeResolver {
   public required ITypeParameterSymbol replacedParameterSymbol { get; init; }
   public required TypeReference replacingType { get; init; }

   public override TypeReference ResolveTypeParameter (ITypeParameterSymbol typeParameterSymbol) {
      if (typeParameterSymbol.Equals(replacedParameterSymbol, SymbolEqualityComparer.Default)) {
         return replacingType;
      }

      return base.ResolveTypeParameter(typeParameterSymbol);
   }
}
