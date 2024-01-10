using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

static class TypeReferenceFactory {
   public static TypeParameter CreateTypeParameter (ITypeParameterSymbol typeParameterSymbol) {
      return new() { symbol = typeParameterSymbol };
   }

   public static CommonTypeReference CreateCommonType (ITypeSymbol typeSymbol) {
      return new() { symbol = typeSymbol };
   }
}
