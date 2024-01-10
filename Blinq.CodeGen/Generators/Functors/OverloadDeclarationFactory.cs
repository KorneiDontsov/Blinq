using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Blinq.CodeGen.CodeElements;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.Functors;

sealed class OverloadDeclarationFactory {
   readonly OriginInfo originInfo;
   readonly ImmutableArray<ITypeParameterSymbol> overloadTypeParameterSymbols;
   readonly OverloadBody overloadBody;

   OverloadInfo? overloadInfo;
   ParameterReplacingTypeResolver? typeResolver;

   public OverloadDeclarationFactory (OriginInfo originInfo) {
      this.originInfo = originInfo;
      overloadTypeParameterSymbols =
         originInfo.signature.symbol
            .TypeParameters
            .Remove(originInfo.functorTypeParameterSymbol);
      overloadBody =
         new OverloadBody {
            targetParameterIndex =
               originInfo.signature.symbol
                  .Parameters
                  .IndexOf(originInfo.functorParameterSymbol),
         };
      Debug.Assert(overloadBody.targetParameterIndex >= 0);
   }

   TypeParameterConstraint CreateConstraint (ITypeParameterSymbol typeParameterSymbol) {
      return new TypeParameterConstraint {
         typeParameter = new() { symbol = typeParameterSymbol },
         constraintTypes =
            typeParameterSymbol.ConstraintTypes
               .ToValueList()
               .ConvertAll(typeResolver!.resolve),
      };
   }

   Parameter CreateParameter (IParameterSymbol parameterSymbol) {
      var isFunctorParameter =
         parameterSymbol.Equals(originInfo.functorParameterSymbol, SymbolEqualityComparer.Default);
      return new Parameter {
         name = parameterSymbol.Name,
         type =
            isFunctorParameter
               ? overloadInfo!.functorProtoType
               : typeResolver!.Resolve(parameterSymbol.Type),
      };
   }

   public MethodDeclaration Create (OverloadInfo overloadInfo) {
      this.overloadInfo = overloadInfo;
      typeResolver =
         new ParameterReplacingTypeResolver {
            replacedParameterSymbol = originInfo.functorTypeParameterSymbol,
            replacingType = overloadInfo.functorImplType,
         };
      try {
         return new MethodDeclaration {
            signature = originInfo.signature,
            typeParameters =
               overloadTypeParameterSymbols
                  .ToValueList()
                  .ConvertAll(TypeReferenceFactory.CreateTypeParameter),
            constraints =
               overloadTypeParameterSymbols
                  .Where(typeParameterSymbol => typeParameterSymbol.ConstraintTypes.Length > 0)
                  .Select(CreateConstraint)
                  .ToArray(),
            parameters =
               originInfo.signature.symbol
                  .Parameters
                  .ToValueList()
                  .ConvertAll(CreateParameter),
            returnType = typeResolver.Resolve(originInfo.signature.symbol.ReturnType),
            body = overloadBody,
         };
      } finally {
         this.overloadInfo = null;
         typeResolver = null;
      }
   }
}
