using System.Threading;
using Blinq.CodeGen.CodeElements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Blinq.CodeGen.Functors;

[Generator]
public class OverloadGenerator: IIncrementalGenerator {
   static bool MatchFunctorParameter (SyntaxNode syntax, CancellationToken ct) {
      return syntax is ParameterSyntax {
         Type: GenericNameSyntax {
            Identifier.ValueText: Identifiers.Pin,
            TypeArgumentList.Arguments: [
               GenericNameSyntax { Identifier.ValueText: Identifiers.IFunctor },
               IdentifierNameSyntax,
            ],
         },
         Parent.Parent.Parent: ClassDeclarationSyntax containingType,
      } && containingType.Modifiers.Any(SyntaxKind.PartialKeyword);
   }

   static OriginInfo? SelectOriginInfo (
      GeneratorSyntaxContext context,
      CancellationToken ct
   ) {
      var targetSymbol = context.SemanticModel.GetDeclaredSymbol(context.Node, ct);
      if (
         targetSymbol is IParameterSymbol {
            Type: INamedTypeSymbol {
               Name: Identifiers.Pin,
               TypeArguments: [
                  INamedTypeSymbol {
                     Name: Identifiers.IFunctor,
                     TypeArguments: var functorTypeArguments,
                  },
                  ITypeParameterSymbol functorTypeParameterSymbol,
               ],
            },
            ContainingSymbol: IMethodSymbol methodSymbol,
         } functorParameterSymbol
      ) {
         return new OriginInfo {
            signature = new() { symbol = methodSymbol },
            functorTypeArgSymbols = functorTypeArguments,
            functorTypeParameterSymbol = functorTypeParameterSymbol,
            functorParameterSymbol = functorParameterSymbol,
         };
      }

      return null;
   }

   static CodeFile CreateOverloadFile (
      OriginInfo originInfo,
      CancellationToken ct
   ) {
      var typeArgs = originInfo.functorTypeArgSymbols.ConvertAll(TypeResolver.@default.resolve);

      ReadOnlyValueList<OverloadInfo> overloadMethodInfos =
         typeArgs.Count switch {
            > 1 => new[] {
               OverloadInfoFactory.NewSystemFuncOverload(typeArgs),
               OverloadInfoFactory.NewByRefFuncOverload(typeArgs),
            },
            _ => new[] {
               OverloadInfoFactory.NewSystemFuncOverload(typeArgs),
            },
         };

      return new CodeFile {
         fileName = CodeGenUtils.CreateFileName(originInfo.signature.symbol, typeArgs),
         @namespace = originInfo.signature.symbol.ContainingNamespace,
         declarations = new Declaration[] {
            new TypeDeclaration {
               name = originInfo.signature.symbol.ContainingType.Name,
               accessibility = originInfo.signature.symbol.ContainingType.DeclaredAccessibility,
               modifiers =
                  TypeModifiers.Unsafe
                  | TypeModifiers.Partial
                  | originInfo.signature.symbol.ContainingType.GetTypeModifiers(),
               members =
                  overloadMethodInfos.ConvertAll<Declaration>(
                     new OverloadDeclarationFactory(originInfo).Create
                  ),
            },
         },
      };
   }

   public void Initialize (IncrementalGeneratorInitializationContext context) {
      var overloadFiles =
         context.SyntaxProvider
            .CreateSyntaxProvider(MatchFunctorParameter, SelectOriginInfo)
            .WhereNotNull()
            .Select(CreateOverloadFile);
      context.RegisterSourceOutput(overloadFiles);
   }
}
