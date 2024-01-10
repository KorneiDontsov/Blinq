using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices {
   [ExcludeFromCodeCoverage]
   [DebuggerNonUserCode]
   [AttributeUsage(
      AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field
      | AttributeTargets.Property,
      Inherited = false
   )]
   sealed class RequiredMemberAttribute: Attribute { }

   [ExcludeFromCodeCoverage]
   [DebuggerNonUserCode]
   [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
   sealed class CompilerFeatureRequiredAttribute: Attribute {
      public CompilerFeatureRequiredAttribute (string featureName) {
         FeatureName = featureName;
      }

      public string FeatureName { get; }

      public bool
         IsOptional {
         get;
         set;
      } // Originally, this was 'Init', but that does not seem necessary and may collide with the IsExternalInit package

      public const string RefStructs = nameof(RefStructs);
      public const string RequiredMembers = nameof(RequiredMembers);
   }
}

namespace System.Diagnostics.CodeAnalysis {
   [ExcludeFromCodeCoverage]
   [DebuggerNonUserCode]
   [AttributeUsage(AttributeTargets.Constructor)]
   sealed class SetsRequiredMembersAttribute: Attribute { }
}
