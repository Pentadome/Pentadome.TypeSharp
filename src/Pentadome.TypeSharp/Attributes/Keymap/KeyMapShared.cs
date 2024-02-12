using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pentadome.TypeSharp.Extensions;
using Pentadome.TypeSharp.Models;

namespace Pentadome.TypeSharp.Attributes.Keymap;

internal static class KeyMapShared
{
    internal const string _defaultSuffix = "KeyMap";
    internal const string _generatedKeyMapAccessibility = "GeneratedKeyMapAccessibility";

    internal static string? GetKeyMapNameArgument(
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax,
        CancellationToken cancellationToken = default
    )
    {
        var nameArgument = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.Identifier.ValueText == nameof(GenerateKeyMapOfAttribute.Name)
        );

        if (nameArgument is null)
            return null;

        var constantValue = semanticModel.GetConstantValue(
            nameArgument.Expression,
            cancellationToken
        );

        return (string?)constantValue.Value;
    }

    internal static string? GetKeyMapNameSpaceArgument(
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax,
        CancellationToken cancellationToken = default
    )
    {
        var nameArgument = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.Identifier.ValueText == nameof(GenerateKeyMapOfAttribute.NameSpace)
        );

        if (nameArgument is null)
            return null;

        var constantValue = semanticModel.GetConstantValue(
            nameArgument.Expression,
            cancellationToken
        );

        return (string?)constantValue.Value;
    }

    internal static string GetEnumValuesWithoutExcludesDeclaration(
        SourceProductionContext context,
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax,
        INamedTypeSymbol mappedType
    )
    {
        var properties = mappedType
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Select(x => x.Name)
            .ToArray();

        var excludes = GetExcludeArgument(semanticModel, attributeSyntax);

        ValidateExcludes(context, mappedType.ToDisplayString(), properties, excludes);

        var enums = string.Join(",\n        ", properties.Except(excludes.Select(x => x.Value)));
        return enums;
    }

    private static void ValidateExcludes(
        SourceProductionContext context,
        string typeName,
        string[] properties,
        ArrayValue<string?>[] excludes
    )
    {
        var invalidExcludes = excludes.Where(x => !properties.Contains(x.Value)).ToArray();

        foreach (var invalidExclude in invalidExcludes)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics._keyMapOfAttributeExcludeArgumentIsNotAnProperty,
                    invalidExclude.Location,
                    invalidExclude.Value,
                    typeName
                )
            );
        }
    }

    private static ArrayValue<string?>[] GetExcludeArgument(
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax
    )
    {
        var excludeArgument = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.Identifier.ValueText == nameof(GenerateKeyMapOfAttribute.Exclude)
        );

        return excludeArgument is null
            ? []
            : semanticModel.GetConstantArray<string>(excludeArgument.Expression);
    }

    internal static string GetAccessibility(
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax,
        ISymbol mappedType
    )
    {
        var accessibilityArgument = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.Identifier.ValueText
            == nameof(GenerateKeyMapOfAttribute.Accessibility)
        );

        var accessibility = accessibilityArgument is null
            ? 0
            : semanticModel.GetConstantOrDefault<int>(accessibilityArgument.Expression);

        return accessibility switch
        {
            // convert the enums to int during compile time.
            (int)GeneratedKeyMapAccessibility.Target
                => mappedType.DeclaredAccessibility == Accessibility.Internal
                    ? "internal"
                    : "public",
            (int)GeneratedKeyMapAccessibility.Internal => "internal",
            (int)GeneratedKeyMapAccessibility.Public => "public",
            _
                => throw new InvalidOperationException(
                    $"unexpected value for {nameof(accessibility)}: {accessibility}"
                )
        };
    }
}
