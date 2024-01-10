using System.Text;

namespace Blinq.CodeGen.CodeElements;

sealed record TypeParameterConstraint: CodeElement {
   public required TypeParameter typeParameter { get; init; }
   public required ReadOnlyValueList<TypeReference> constraintTypes { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      code.Append("where ");
      typeParameter.AppendTo(ref code, in context);
      code.Append(": ");
      constraintTypes.AppendAllTo(ref code, in context);
   }
}
