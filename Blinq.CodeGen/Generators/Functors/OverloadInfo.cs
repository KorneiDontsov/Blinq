using Blinq.CodeGen.CodeElements;

namespace Blinq.CodeGen.Functors;

sealed class OverloadInfo {
   public required TypeReference functorImplType { get; init; }
   public required TypeReference functorProtoType { get; init; }
}
