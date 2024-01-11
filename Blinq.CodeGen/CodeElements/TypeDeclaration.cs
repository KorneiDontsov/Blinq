using System.Text;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen.CodeElements;

sealed record TypeDeclaration: Declaration {
   public required string name { get; init; }
   public required Accessibility accessibility { get; init; }
   public required TypeModifiers modifiers { get; init; }
   public required ReadOnlyValueList<Declaration> members { get; init; }

   public override void AppendTo (ref ValueStringBuilder code, in CodeGenContext context) {
      context.indent.AppendTo(ref code);

      if (this.accessibility != Accessibility.NotApplicable) {
         code.Append(this.accessibility.AsKeywordText());
         code.Append(' ');
      }

      if ((this.modifiers & TypeModifiers.Static) != 0) {
         code.Append("static ");
      }

      if ((this.modifiers & TypeModifiers.Unsafe) != 0) {
         code.Append("unsafe ");
      }

      if ((this.modifiers & TypeModifiers.Partial) != 0) {
         code.Append("partial ");
      }

      if ((this.modifiers & TypeModifiers.Record) != 0) {
         code.Append("record ");
      }

      code.Append((this.modifiers & TypeModifiers.Struct) != 0 ? "struct" : "class");
      code.Append(' ');
      code.Append(this.name);
      code.Append(" {\n");

      var memberContext = context with { indent = context.indent.Child() };
      foreach (var member in this.members) {
         member.AppendTo(ref code, in memberContext);
      }

      context.indent.AppendTo(ref code);
      code.Append("}\n");
   }
}
