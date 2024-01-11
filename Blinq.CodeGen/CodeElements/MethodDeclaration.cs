using System.Text;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

sealed record MethodDeclaration: Declaration {
   public abstract record Body {
      public abstract void AppendTo (
         ref ValueStringBuilder code,
         in CodeGenContext context,
         MethodDeclaration declaration
      );
   }

   public required MethodSignature signature { get; init; }
   public required ReadOnlyValueList<TypeParameter> typeParameters { get; init; }
   public required ReadOnlyValueList<TypeParameterConstraint> constraints { get; init; }
   public required ReadOnlyValueList<Parameter> parameters { get; init; }
   public required TypeReference returnType { get; init; }
   public required Body body { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      context.indent.AppendTo(ref code);

      var accessibility = this.signature.accessibility;
      if (accessibility != Accessibility.NotApplicable) {
         code.Append(accessibility.AsKeywordText());
         code.Append(' ');
      }

      var modifiers = this.signature.modifiers;
      if ((modifiers & MethodModifiers.Static) != 0) {
         code.Append("static ");
      }

      this.returnType.AppendTo(ref code, in context);
      code.Append(' ');
      code.Append(this.signature.name);

      if (this.typeParameters.Count > 0) {
         code.Append(context.symbols.openGenericBracket);
         this.typeParameters.AppendAllTo(ref code, in context);
         code.Append(context.symbols.closedGenericBracket);
      }

      code.Append(" (");

      if (this.parameters.Count > 0) {
         if ((modifiers & MethodModifiers.Extension) != 0) {
            code.Append("this ");
         }

         this.parameters.AppendAllTo(ref code, in context);
      }

      code.Append(')');

      foreach (var constraint in this.constraints) {
         code.Append('\n');
         context.indent.AppendTo(ref code);
         constraint.AppendTo(ref code, in context);
      }

      code.Append(" {\n");

      this.body.AppendTo(ref code, context with { indent = context.indent.Child() }, this);

      context.indent.AppendTo(ref code);
      code.Append("}\n");
   }
}
