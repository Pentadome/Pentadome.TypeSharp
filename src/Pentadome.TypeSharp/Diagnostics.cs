using Microsoft.CodeAnalysis;

namespace Pentadome.TypeSharp;

internal static class Diagnostics
{
    internal static readonly DiagnosticDescriptor _keyMapOfAttributeExcludeArgumentIsNotAnProperty =
        new(
            "TypeSharp1001",
            "Exclude argument should be name of a property",
            "The exclude argument \"{0}\" is not a property of \"{1}\" and will be ignored",
            "Pentadome.TypeSharp",
            DiagnosticSeverity.Warning,
            true
        );
}
