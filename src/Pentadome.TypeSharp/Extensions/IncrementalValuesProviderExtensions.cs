using Microsoft.CodeAnalysis;

namespace Pentadome.TypeSharp.Extensions;

internal static class IncrementalValuesProviderExtensions
{
    internal static IncrementalValuesProvider<T> Flatten<T>(
        this IncrementalValuesProvider<IEnumerable<T>> @this
    )
    {
        return @this.SelectMany((x, _) => x);
    }
}
