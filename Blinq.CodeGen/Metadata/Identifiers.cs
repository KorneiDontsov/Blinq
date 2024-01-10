using System;
using System.Diagnostics.CodeAnalysis;

namespace Blinq.CodeGen;

[SuppressMessage("ReSharper", "InconsistentNaming")]
static class Identifiers {
   public const string System = nameof(System);
   public const string Func = nameof(Func<object>);

   public const string Pin = nameof(Pin);
   public const string IFunctor = nameof(IFunctor);
   public const string Functor = nameof(Functor);
   public const string ByRefFunctor = nameof(ByRefFunctor);
   public const string ByRefFunc = nameof(ByRefFunc);

   public const string New = nameof(New);

   public const string value = nameof(value);
}
