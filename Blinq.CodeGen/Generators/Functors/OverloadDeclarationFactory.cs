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
      this.overloadTypeParameterSymbols =
         originInfo.signature.symbol
            .TypeParameters
            .Remove(originInfo.functorTypeParameterSymbol);
      this.overloadBody =
         new OverloadBody {
            targetParameterIndex =
               originInfo.signature.symbol
                  .Parameters
                  .IndexOf(originInfo.functorParameterSymbol),
         };
      Debug.Assert(this.overloadBody.targetParameterIndex >= 0);
   }

   TypeParameterConstraint CreateConstraint (ITypeParameterSymbol typeParameterSymbol) {
      return new TypeParameterConstraint {
         typeParameter = new() { symbol = typeParameterSymbol },
         constraintTypes =
            typeParameterSymbol.ConstraintTypes
               .ToValueList()
               .ConvertAll(this.typeResolver!.resolve),
      };
   }

   Parameter CreateParameter (IParameterSymbol parameterSymbol) {
      var isFunctorParameter =
         parameterSymbol.Equals(
            this.originInfo.functorParameterSymbol,
            SymbolEqualityComparer.Default
         );
      return new Parameter {
         name = parameterSymbol.Name,
         type =
            isFunctorParameter switch {
               true => this.overloadInfo!.functorProtoType,
               false => this.typeResolver!.Resolve(parameterSymbol.Type),
            },
      };
   }

   public MethodDeclaration Create (OverloadInfo overloadInfo) {
      this.overloadInfo = overloadInfo;
      this.typeResolver =
         new ParameterReplacingTypeResolver {
            replacedParameterSymbol = this.originInfo.functorTypeParameterSymbol,
            replacingType = overloadInfo.functorImplType,
         };
      try {
         return new MethodDeclaration {
            signature = this.originInfo.signature,
            typeParameters =
               this.overloadTypeParameterSymbols
                  .ToValueList()
                  .ConvertAll(TypeReferenceFactory.CreateTypeParameter),
            constraints =
               this.overloadTypeParameterSymbols
                  .Where(typeParameterSymbol => typeParameterSymbol.ConstraintTypes.Length > 0)
                  .Select(this.CreateConstraint)
                  .ToArray(),
            parameters =
               this.originInfo.signature.symbol
                  .Parameters
                  .ToValueList()
                  .ConvertAll(this.CreateParameter),
            returnType = this.typeResolver.Resolve(this.originInfo.signature.symbol.ReturnType),
            body = this.overloadBody,
         };
      } finally {
         this.overloadInfo = null;
         this.typeResolver = null;
      }
   }
}
