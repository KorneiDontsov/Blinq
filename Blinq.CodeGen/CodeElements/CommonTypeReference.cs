using System.Text;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

sealed record CommonTypeReference: TypeReference {
   public required ITypeSymbol symbol { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      symbol.AppendFullQualifiedNameTo(ref code, in context);
   }
}
