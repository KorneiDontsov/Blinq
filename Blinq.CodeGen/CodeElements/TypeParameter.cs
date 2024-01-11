using System.Text;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

sealed record TypeParameter: TypeReference {
   public required ITypeParameterSymbol symbol { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      code.Append(this.symbol.Name);
   }
}
