using Microsoft.CodeAnalysis;

namespace Pentadome.TypeSharp;

internal static class Diagnostics
{
    private static readonly DiagnosticDescriptor _keyMapOfAttributeExcludeArgumentIsNotAnProperty =
        new(
            "TypeSharp1001",
            "Exclude argument should be name of a property",
            "The exclude argument \"{0}\" is not a property of \"{1}\" and will be ignored",
            Constants._assemblyName,
            DiagnosticSeverity.Warning,
            true
        );

    internal static void ReportKeyMapOfAttributeExcludeArgumentIsNotAnProperty(
        this SourceProductionContext @this,
        Location? location,
        string excludeArgument,
        string typeName
    ) =>
        @this.ReportDiagnostic(
            Diagnostic.Create(
                _keyMapOfAttributeExcludeArgumentIsNotAnProperty,
                location,
                excludeArgument,
                typeName
            )
        );
}
