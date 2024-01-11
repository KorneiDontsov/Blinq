using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen;

readonly struct MethodSignature: IEquatable<MethodSignature> {
   readonly IMethodSymbol _symbol;
   readonly MethodModifiers _modifiers;

   public required IMethodSymbol symbol {
      get => this._symbol;
      init {
         this._symbol = value;
         this._modifiers = value.GetMethodModifiers();
      }
   }

   public MethodModifiers modifiers => this._modifiers;
   public Accessibility accessibility => this.symbol.DeclaredAccessibility;
   public string name => this.symbol.Name;

   public bool Equals (MethodSignature other) {
      return this.symbol.Equals(other.symbol, SymbolEqualityComparer.Default)
         && this.symbol.Parameters.SequenceEqual(other.symbol.Parameters)
         && this.symbol.DeclaredAccessibility == other.symbol.DeclaredAccessibility
         && this.modifiers == other.modifiers;
   }

   public override bool Equals (object? obj) {
      return obj is MethodSignature other && this.Equals(other);
   }

   public override int GetHashCode () {
      throw new NotSupportedException();
   }

   public override string ToString () {
      return this.symbol.ToString();
   }
}
