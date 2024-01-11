using System.Text;

namespace Blinq.CodeGen;

readonly struct SyntaxIndent {
   static readonly string[] indentStrings = {
      string.Empty,
      "   ",
      "      ",
      "         ",
   };

   static string CreateIndentString (uint value) {
      var str = new StringBuilder();
      for (uint i = 0; i < value; ++i) {
         str.Append("   ");
      }

      return str.ToString();
   }

   readonly string str;
   readonly uint value;

   SyntaxIndent (string str, uint value) {
      this.str = str;
      this.value = value;
   }

   SyntaxIndent (uint value) {
      this.str = value < (uint)indentStrings.Length
         ? indentStrings[value]
         : CreateIndentString(value);
      this.value = value;
   }

   public static SyntaxIndent none => new(string.Empty, 0);

   public SyntaxIndent Child () {
      return new(this.value + 1);
   }

   public void AppendTo (ref ValueStringBuilder code) {
      code.Append(this.str);
   }

   public override string ToString () {
      return this.str;
   }
}
