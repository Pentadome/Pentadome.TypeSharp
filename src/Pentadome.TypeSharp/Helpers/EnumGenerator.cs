using Pentadome.TypeSharp.Models;

namespace Pentadome.TypeSharp.Helpers;

internal static class EnumGenerator
{
    internal static string GenerateEnum(
        string nameSpace,
        string name,
        string accessibility,
        IReadOnlyList<EnumValueDeclaration> enumValues,
        bool isFlagEnum,
        string? comment = null
    )
    {
        var syntaxBuilder = new SimpleAutoFormattingSyntaxBuilder()
            .NameSpaceBracketOpenLine(nameSpace)
            .XmlDocLine(comment)
            .EnumBracketOpenLine(name, accessibility, isFlagEnum);

        if (isFlagEnum)
            AppendFlagEnumValues(syntaxBuilder, enumValues);
        else
            AppendEnumValues(syntaxBuilder, enumValues);

        return syntaxBuilder.BracketCloseLine().BracketCloseLine().ToString();
    }

    private static void AppendEnumValues(
        SimpleAutoFormattingSyntaxBuilder stringBuilder,
        IReadOnlyList<EnumValueDeclaration> values
    )
    {
        foreach (var (value, comment) in values)
        {
            _ = stringBuilder.XmlDocLine(comment).Append(value).AppendLine(",");
        }
    }

    private static void AppendFlagEnumValues(
        SimpleAutoFormattingSyntaxBuilder stringBuilder,
        IReadOnlyList<EnumValueDeclaration> values
    )
    {
        _ = stringBuilder.XmlDocLine("Represents no value.").AppendLine("None = 0,");

        for (var i = 0; i < values.Count; i++)
        {
            var (value, comment) = values[i];

            _ = stringBuilder
                .XmlDocLine(comment)
                .Append(value)
                .Append(" = 1 << ")
                .Append(i)
                .AppendLine(",");
        }

        _ = stringBuilder
            .XmlDocLine("Represent the selection of all value flags.")
            .Append("All = (1 << ")
            .Append(values.Count)
            .AppendLine(") - 1");
    }
}
