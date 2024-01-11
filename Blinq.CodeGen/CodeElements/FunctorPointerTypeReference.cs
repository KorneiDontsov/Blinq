using System.Text;

namespace Blinq.CodeGen.CodeElements;

sealed record FunctorPointerTypeReference: TypeReference {
   public required ReadOnlyValueList<TypeReference> parameterTypes { get; init; }
   public TypeReference? returnType { get; init; }


   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      code.Append("delegate*");
      code.Append(context.symbols.openGenericBracket);
      if (this.parameterTypes.Count > 0) {
         this.parameterTypes.AppendAllTo(ref code, in context);
         code.Append(context.symbols.listSeparator);
      }

      if (this.returnType is not null) {
         this.returnType.AppendTo(ref code, in context);
      } else {
         code.Append("void");
      }

      code.Append(context.symbols.closedGenericBracket);
   }
}
