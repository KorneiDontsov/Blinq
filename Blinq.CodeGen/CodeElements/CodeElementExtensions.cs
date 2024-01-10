using System.Text;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

static class CodeElementExtensions {
   public static void AppendAllTo<TCodeElement> (
      this ReadOnlyValueList<TCodeElement> codeElements,
      ref ValueStringBuilder code,
      in CodeGenContext context
   ) where TCodeElement: CodeElement {
      if (codeElements.Count == 0) return;

      codeElements[0].AppendTo(ref code, in context);

      var listSeparator = context.symbols.listSeparator;
      for (var index = 1; index < codeElements.Count; index++) {
         code.Append(listSeparator);
         codeElements[index].AppendTo(ref code, in context);
      }
   }

   public static string AsKeywordText (this Accessibility accessibility) {
      return accessibility switch {
         Accessibility.Private => "private",
         Accessibility.Internal => "internal",
         Accessibility.Protected => "protected",
         Accessibility.ProtectedAndInternal => "private protected",
         Accessibility.ProtectedOrInternal => "protected internal",
         Accessibility.Public => "public",
         _ => string.Empty,
      };
   }
}
