using System.Text;

namespace Blinq.CodeGen.CodeElements;

sealed record Parameter: CodeElement {
   public required string name { get; init; }
   public required TypeReference type { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      type.AppendTo(ref code, in context);
      code.Append(' ');
      code.Append(name);
   }
}
