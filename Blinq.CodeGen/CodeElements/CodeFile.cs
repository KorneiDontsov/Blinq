using System.Text;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

sealed record CodeFile {
   public required string fileName { get; init; }
   public INamespaceSymbol? @namespace { get; init; }
   public required ReadOnlyValueList<Declaration> declarations { get; init; }

   public override string ToString () {
      var code = new ValueStringBuilder(stackalloc char[1024]);
      try {
         var context = new CodeGenContext {
            indent = SyntaxIndent.none,
            currentNamespace = @namespace,
         };

         if (@namespace is { IsGlobalNamespace: false }) {
            code.Append("namespace ");
            @namespace.AppendFullQualifiedNameTo(ref code, in context);
            code.Append(";\n");
         }

         foreach (var declaration in declarations) {
            declaration.AppendTo(ref code, context);
         }

         return code.ToString();
      } finally {
         code.Dispose();
      }
   }
}
