using System.Text;

namespace Blinq.CodeGen.CodeElements;

sealed record GenericTypeReference: TypeReference {
   public required TypeReference openType { get; init; }
   public required ReadOnlyValueList<TypeReference> typeArguments { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      openType.AppendTo(ref code, in context);

      code.Append(context.symbols.openGenericBracket);
      typeArguments.AppendAllTo(ref code, in context);
      code.Append(context.symbols.closedGenericBracket);
   }
}
