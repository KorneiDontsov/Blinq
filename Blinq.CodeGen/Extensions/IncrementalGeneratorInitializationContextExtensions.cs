using Blinq.CodeGen.CodeElements;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen;

static class IncrementalGeneratorInitializationContextExtensions {
   public static void RegisterSourceOutput (
      this in IncrementalGeneratorInitializationContext context,
      IncrementalValuesProvider<CodeFile> fileSyntax
   ) {
      context.RegisterSourceOutput(
         fileSyntax,
         static (context, fileSyntax) => {
            context.AddSource(fileSyntax.fileName, fileSyntax.ToString());
         }
      );
   }
}
