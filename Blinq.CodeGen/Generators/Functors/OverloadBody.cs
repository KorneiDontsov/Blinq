using System.Text;
using Blinq.CodeGen.CodeElements;

namespace Blinq.CodeGen.Functors;

sealed record OverloadBody: MethodDeclaration.Body {
   public required int targetParameterIndex { get; init; }

   public override void AppendTo (
      ref ValueStringBuilder code,
      in CodeGenContext context,
      MethodDeclaration declaration
   ) {
      context.indent.AppendTo(ref code);
      code.Append("return ");
      code.Append(declaration.signature.symbol.Name);
      code.Append('(');

      var parameters = declaration.parameters;
      for (var parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++) {
         var parameterName = parameters[parameterIndex].name;

         if (parameterIndex != targetParameterIndex) {
            code.Append(parameterName);
         } else {
            code.Append($"{Identifiers.Functor}.{Identifiers.New}(");
            code.Append(parameterName);
            code.Append(')');
         }

         code.Append(", ");
      }

      code.Length -= 2;
      code.Append(");\n");
   }
}
