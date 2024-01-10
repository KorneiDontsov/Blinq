using Microsoft.CodeAnalysis;

namespace Blinq.CodeGen;

static class IncrementalValuesProviderExtensions {
   public static IncrementalValuesProvider<T> WhereNotNull<T> (
      this IncrementalValuesProvider<T?> provider
   ) where T: class {
      return provider.Where(item => item is not null)!;
   }
}
