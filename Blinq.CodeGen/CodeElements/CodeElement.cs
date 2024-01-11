using System.Text;

namespace Blinq.CodeGen.CodeElements;

abstract record CodeElement {
   public abstract void AppendTo (ref ValueStringBuilder code, in CodeGenContext context);

   public sealed override string ToString () {
      var code = new ValueStringBuilder(stackalloc char[512]);
      try {
         this.AppendTo(ref code, new CodeGenContext());
         return code.ToString();
      } finally {
         code.Dispose();
      }
   }
}
