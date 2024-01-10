using System;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

class TypeResolver {
   public readonly Func<ITypeSymbol, TypeReference> resolve;

   public virtual TypeReference ResolveNamed (INamedTypeSymbol namedTypeSymbol) {
      var type = TypeReferenceFactory.CreateCommonType(namedTypeSymbol);
      var typeArgumentSymbols = namedTypeSymbol.TypeArguments;
      if (typeArgumentSymbols.Length == 0) return type;

      return new GenericTypeReference {
         openType = type,
         typeArguments = typeArgumentSymbols.ToValueList().ConvertAll(resolve),
      };
   }

   public virtual TypeReference ResolveTypeParameter (ITypeParameterSymbol typeParameterSymbol) {
      return TypeReferenceFactory.CreateTypeParameter(typeParameterSymbol);
   }

   public virtual TypeReference Resolve (ITypeSymbol typeSymbol) {
      return typeSymbol switch {
         INamedTypeSymbol namedType => ResolveNamed(namedType),
         ITypeParameterSymbol typeParameter => ResolveTypeParameter(typeParameter),
         _ => TypeReferenceFactory.CreateCommonType(typeSymbol),
      };
   }

   public TypeResolver () {
      resolve = Resolve;
   }

   public static readonly TypeResolver @default = new();
}
