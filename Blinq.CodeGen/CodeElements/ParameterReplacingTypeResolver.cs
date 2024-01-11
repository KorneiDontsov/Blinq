using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

class ParameterReplacingTypeResolver: TypeResolver {
   public required ITypeParameterSymbol replacedParameterSymbol { get; init; }
   public required TypeReference replacingType { get; init; }

   public override TypeReference ResolveTypeParameter (ITypeParameterSymbol typeParameterSymbol) {
      var isReplacedParameterSymbol =
         typeParameterSymbol.Equals(this.replacedParameterSymbol, SymbolEqualityComparer.Default);
      if (isReplacedParameterSymbol) return this.replacingType;

      return base.ResolveTypeParameter(typeParameterSymbol);
   }
}
