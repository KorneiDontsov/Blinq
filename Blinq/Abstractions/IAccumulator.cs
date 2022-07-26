using System.Diagnostics.CodeAnalysis;

namespace Blinq;

[SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
public interface IAccumulator<T, TAccumulated> {
   bool Invoke (T item, ref TAccumulated accumulated);
}
