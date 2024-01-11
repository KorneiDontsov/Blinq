using System.Text;

namespace Blinq.CodeGen.CodeElements;

sealed record PredefinedTypeReference: TypeReference {
   public required string name { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      code.Append(this.name);
   }
}
