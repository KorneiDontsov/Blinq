using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen;

readonly struct MethodSignature: IEquatable<MethodSignature> {
   readonly IMethodSymbol _symbol;
   readonly MethodModifiers _modifiers;

   public required IMethodSymbol symbol {
      get => _symbol;
      init {
         _symbol = value;
         _modifiers = value.GetMethodModifiers();
      }
   }

   public MethodModifiers modifiers => _modifiers;
   public Accessibility accessibility => symbol.DeclaredAccessibility;
   public string name => symbol.Name;

   public bool Equals (MethodSignature other) {
      return symbol.Equals(other.symbol, SymbolEqualityComparer.Default)
         && symbol.Parameters.SequenceEqual(other.symbol.Parameters)
         && symbol.DeclaredAccessibility == other.symbol.DeclaredAccessibility
         && modifiers == other.modifiers;
   }

   public override bool Equals (object? obj) {
      return obj is MethodSignature other && Equals(other);
   }

   public override int GetHashCode () {
      throw new NotSupportedException();
   }

   public override string ToString () {
      return symbol.ToString();
   }
}
