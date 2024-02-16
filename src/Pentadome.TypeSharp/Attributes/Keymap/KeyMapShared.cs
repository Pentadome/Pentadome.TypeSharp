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

    internal static List<string> GetMappedProperties(
        SourceProductionContext context,
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax,
        INamedTypeSymbol mappedType
    )
    {
        var properties = mappedType
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && !x.IsStatic)
            .Select(x => x.Name)
            .ToList();

        var excludes = GetExcludeArgument(semanticModel, attributeSyntax);

        RemoveExcludesFromPropertyList(properties, excludes, context, mappedType.ToDisplayString());
        return properties;
    }

    /// <summary>
    /// Also reports diagnostics if <paramref name="excludes"/> contains a string not in <paramref name="properties"/>.
    /// </summary>
    private static void RemoveExcludesFromPropertyList(
        List<string> properties,
        IEnumerable<ArrayValue<string?>> excludes,
        SourceProductionContext context,
        string typeName
    )
    {
        foreach (var exclude in excludes)
        {
            if (properties.Remove(exclude.Value!))
                continue;

            // if Remove returned false, it was not a property of the type.
            context.ReportDiagnostic(
                Diagnostic.Create(
                    Diagnostics._keyMapOfAttributeExcludeArgumentIsNotAnProperty,
                    exclude.Location,
                    exclude.Value,
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

    internal static int GetKeyMapKind(SemanticModel semanticModel, AttributeSyntax attributeSyntax)
    {
        var kindArgument = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.Identifier.ValueText == nameof(GenerateKeyMapOfAttribute.Kind)
        );

        return kindArgument is null
            ? (int)GeneratedKeyMapKind.Enum
            : semanticModel.GetConstantOrDefault<int>(kindArgument.Expression);
    }

    internal static IReadOnlyList<EnumValueDeclaration> GenerateEnumValueComments(
        string mappedTypeFullName,
        IReadOnlyList<string> mappedProperties
    )
    {
        var enumValueDeclarations = new EnumValueDeclaration[mappedProperties.Count];

        for (var i = 0; i < mappedProperties.Count; i++)
        {
            var cref = $"{mappedTypeFullName}.{mappedProperties[i]}";
            var comment = $"""
                Represents the property <see cref="{cref}"/>
                <inheritdoc cref="{cref}"/>
                """;

            enumValueDeclarations[i] = new(mappedProperties[i], comment);
        }

        return enumValueDeclarations;
    }

    internal static string GenerateKeyMapComment(string mappedTypeFullName)
    {
        return $"""
            Represents a keymap for <see cref="{mappedTypeFullName}" />.
            """;
    }
}
