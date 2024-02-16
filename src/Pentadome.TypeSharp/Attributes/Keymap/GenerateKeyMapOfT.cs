using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pentadome.TypeSharp.Extensions;
using Pentadome.TypeSharp.Helpers;

namespace Pentadome.TypeSharp.Attributes.Keymap;

internal static class GenerateKeyMapOfT
{
    private const string _generateKeyMapOfAttributeT =
        $"{Constants._assemblyName}.GenerateKeyMapOfAttribute<T>";

    public static void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get attributes that target assembly.
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                (s, _) =>
                    s is AttributeListSyntax x
                    && x.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true,
                GetGenerateKeyOfAttributeSyntaxes
            )
            .Flatten();

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
            GenerateCode(context, compilation, attributeSyntax);
        }
    }

    private static void GenerateCode(
        SourceProductionContext context,
        Compilation compilation,
        AttributeSyntax attributeSyntax
    )
    {
        var semanticModel = compilation.GetSemanticModel(attributeSyntax.SyntaxTree);

        var typeArgument = GetTypeArgument(attributeSyntax);

        if (typeArgument is null)
            return;

        if (semanticModel.GetSymbolInfo(typeArgument).Symbol is not INamedTypeSymbol mappedType)
            return;

        var enumValues = KeyMapShared.GetMappedProperties(
            context,
            semanticModel,
            attributeSyntax,
            mappedType
        );

        var keyMapName =
            KeyMapShared.GetKeyMapNameArgument(semanticModel, attributeSyntax)
            ?? $"{mappedType.Name}{KeyMapShared._defaultSuffix}";

        var keyMapNameSpace =
            KeyMapShared.GetKeyMapNameSpaceArgument(semanticModel, attributeSyntax)
            ?? semanticModel.Compilation.AssemblyName!;

        var accessibility = KeyMapShared.GetAccessibility(
            semanticModel,
            attributeSyntax,
            mappedType
        );

        var keyMapKind = KeyMapShared.GetKeyMapKind(semanticModel, attributeSyntax);
        var isFlagEnum = keyMapKind == (int)GeneratedKeyMapKind.FlagEnum;

        var code = EnumGenerator.GenerateEnum(
            keyMapNameSpace,
            keyMapName,
            accessibility,
            KeyMapShared.GenerateEnumValueComments(mappedType.ToDisplayString(), enumValues),
            isFlagEnum,
            KeyMapShared.GenerateKeyMapComment(mappedType.ToDisplayString(), isFlagEnum)
        );

        context.AddUniqueCsharpSource(
            $"{keyMapNameSpace.Replace('.', '/')}/{keyMapName}",
            SourceText.From(code, Encoding.UTF8)
        );
    }

    private static TypeSyntax? GetTypeArgument(AttributeSyntax attributeSyntax)
    {
        return attributeSyntax.Name is GenericNameSyntax genericName
            ? genericName.TypeArgumentList.Arguments[0]
            : null;
    }
}
