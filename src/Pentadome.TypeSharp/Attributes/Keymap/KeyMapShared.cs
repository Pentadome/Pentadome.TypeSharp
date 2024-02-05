using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pentadome.TypeSharp.Attributes.Keymap;

internal static class KeyMapShared
{
    internal const string _defaultSuffix = "KeyMap";

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

    internal static string[] GetExcludeArgument(
        SemanticModel semanticModel,
        AttributeSyntax attributeSyntax,
        CancellationToken cancellationToken = default
    )
    {
        var excludeArgument = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.Identifier.ValueText == nameof(GenerateKeyMapOfAttribute.Exclude)
        );

        if (excludeArgument is null)
            return [];

        var constantValue = semanticModel.GetConstantValue(
            excludeArgument.Expression,
            cancellationToken
        );

        return (constantValue.Value as string[]) ?? [];
    }

    internal static string GetEnumValuesDeclaration(INamedTypeSymbol mappedType, string[] exclude)
    {
        var properties = mappedType
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Select(x => x.Name)
            .Except(exclude);

        var enums = string.Join(",\n        ", properties);
        return enums;
    }
}
