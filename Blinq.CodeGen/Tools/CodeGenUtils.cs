using System.Text;
using Blinq.CodeGen.CodeElements;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen;

static class CodeGenUtils {
   public static void AppendFullQualifiedNameTo (
      this ISymbol symbol,
      ref ValueStringBuilder code,
      in CodeGenContext context
   ) {
      var containingSymbol = symbol.ContainingSymbol;
      var isInCurrentNamespace =
         containingSymbol.Equals(context.currentNamespace, SymbolEqualityComparer.Default)
         || containingSymbol is INamespaceSymbol { IsGlobalNamespace: true };
      if (isInCurrentNamespace == false) {
         AppendFullQualifiedNameTo(containingSymbol, ref code, in context);
         code.Append('.');
      }

      code.Append(symbol.Name);
   }

   public static string CreateFileName<TCodeElement> (
      ISymbol targetSymbol,
      ReadOnlyValueList<TCodeElement> paramTypes
   ) where TCodeElement: CodeElement {
      var fileNameBuilder = new ValueStringBuilder(stackalloc char[256]);
      try {
         targetSymbol.AppendFullQualifiedNameTo(ref fileNameBuilder, new CodeGenContext());

         if (paramTypes.Count > 0) {
            fileNameBuilder.Append('{');

            var paramContext =
               new CodeGenContext {
                  symbols = CodeGenSymbols.titleize,
                  currentNamespace = targetSymbol.ContainingNamespace,
               };
            paramTypes.AppendAllTo(ref fileNameBuilder, in paramContext);

            fileNameBuilder.Append('}');
         }

         fileNameBuilder.Append(".cs");
         return fileNameBuilder.ToString();
      } finally {
         fileNameBuilder.Dispose();
      }
   }

   public static MethodModifiers GetMethodModifiers (this IMethodSymbol method) {
      var modifiers = MethodModifiers.None;
      if (method.IsStatic) modifiers |= MethodModifiers.Static;
      if (method.IsExtensionMethod) modifiers |= MethodModifiers.Extension;
      return modifiers;
   }

   public static TypeModifiers GetTypeModifiers (this ITypeSymbol type) {
      var modifiers = TypeModifiers.None;
      if (type.IsStatic) modifiers |= TypeModifiers.Static;
      if (type.IsValueType) modifiers |= TypeModifiers.Struct;
      if (type.IsRecord) modifiers |= TypeModifiers.Record;
      return modifiers;
   }
}
