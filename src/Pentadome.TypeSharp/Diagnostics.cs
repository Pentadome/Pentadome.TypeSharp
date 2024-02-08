using Microsoft.CodeAnalysis;

namespace Pentadome.TypeSharp;

internal static class Diagnostics
{
    internal static readonly DiagnosticDescriptor _keyMapOfAttributeExcludeArgumentIsNotAnProperty =
        new(
            "TS1001",
            "Exclude argument should be name of a property",
            "The exclude argument \"{0}\" is not a property of \"{1}\"",
            "Pentadome.TypeSharp",
            DiagnosticSeverity.Warning,
            true
        );
}
