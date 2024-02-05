﻿using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pentadome.TypeSharp.Extensions;

namespace Pentadome.TypeSharp.Attributes.Keymap;

internal static class GenerateKeyMapOfT
{
    private const string _generateKeyMapOfAttributeT =
        $"{Constants.AssemblyName}.GenerateKeyMapOfAttribute<T>";

    public static void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Filter classes annotated with the [Report] attribute. Only filtered Syntax Nodes can trigger code generation.
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                (s, _) =>
                    s is AttributeListSyntax x
                    && x.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true,
                GetGenerateKeyOfAttributeSyntaxes
            )
            .SelectMany((x, _) => x);

        // Generate the source code.
        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right)
        );
    }

    private static IEnumerable<AttributeSyntax> GetGenerateKeyOfAttributeSyntaxes(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken
    )
    {
        var attributeListSyntax = (AttributeListSyntax)context.Node;

        foreach (var attributeSyntax in attributeListSyntax.Attributes)
        {
            var attributeName = context
                .SemanticModel.GetTypeInfo(attributeSyntax, cancellationToken)
                .Type?.OriginalDefinition.ToDisplayString();

            if (attributeName == _generateKeyMapOfAttributeT)
                yield return attributeSyntax;
        }
    }

    private static void GenerateCode(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<AttributeSyntax> attributeSyntaxes
    )
    {
        foreach (var attributeSyntax in attributeSyntaxes)
        {
            var semanticModel = compilation.GetSemanticModel(attributeSyntax.SyntaxTree);

            var typeArgument = GetTypeArgument(attributeSyntax);

            if (typeArgument is null)
                continue;

            if (semanticModel.GetSymbolInfo(typeArgument).Symbol is not INamedTypeSymbol mappedType)
                continue;

            var exclude = KeyMapShared.GetExcludeArgument(semanticModel, attributeSyntax);

            var enums = KeyMapShared.GetEnumValuesDeclaration(mappedType, exclude);

            var keyMapName =
                KeyMapShared.GetKeyMapNameArgument(semanticModel, attributeSyntax)
                ?? $"{mappedType.Name}{KeyMapShared._defaultSuffix}";

            var keyMapNameSpace =
                KeyMapShared.GetKeyMapNameSpaceArgument(semanticModel, attributeSyntax)
                ?? semanticModel.Compilation.AssemblyName!;

            var code = $$"""
namespace {{keyMapNameSpace}}
{
    public enum {{keyMapName}}
    {
        {{enums}}
    }
}
""";
            context.AddUniqueCsharpSource(
                $"{keyMapNameSpace.Replace('.', '/')}/{keyMapName}",
                SourceText.From(code, Encoding.UTF8)
            );
        }
    }

    private static TypeSyntax? GetTypeArgument(AttributeSyntax attributeSyntax)
    {
        return attributeSyntax.Name is GenericNameSyntax genericName
            ? genericName.TypeArgumentList.Arguments[0]
            : null;
    }
}
