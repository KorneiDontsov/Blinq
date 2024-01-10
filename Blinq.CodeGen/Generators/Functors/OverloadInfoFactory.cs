using System.Diagnostics;
using Blinq.CodeGen.CodeElements;

namespace Blinq.CodeGen.Functors;

static class OverloadInfoFactory {
   static readonly PredefinedTypeReference systemFuncType =
      new() { name = $"{Identifiers.System}.{Identifiers.Func}" };

   static readonly PredefinedTypeReference functorType =
      new() { name = Identifiers.Functor };

   static readonly PredefinedTypeReference byRefFunctorType =
      new() { name = Identifiers.ByRefFunctor };

   static readonly PredefinedTypeReference byRefFuncType =
      new() { name = Identifiers.ByRefFunc };

   public static OverloadInfo NewSystemFuncOverload (
      ReadOnlyValueList<TypeReference> typeArguments
   ) {
      Debug.Assert(typeArguments.Count > 0);

      return new OverloadInfo {
         functorImplType = new GenericTypeReference {
            openType = functorType,
            typeArguments = typeArguments,
         },
         functorProtoType = new GenericTypeReference {
            openType = systemFuncType,
            typeArguments = typeArguments,
         },
      };
   }

   public static OverloadInfo NewByRefFuncOverload (
      ReadOnlyValueList<TypeReference> typeArguments
   ) {
      Debug.Assert(typeArguments.Count > 1);

      return new OverloadInfo {
         functorImplType = new GenericTypeReference {
            openType = byRefFunctorType,
            typeArguments = typeArguments,
         },
         functorProtoType = new GenericTypeReference {
            openType = byRefFuncType,
            typeArguments = typeArguments,
         },
      };
   }
}
