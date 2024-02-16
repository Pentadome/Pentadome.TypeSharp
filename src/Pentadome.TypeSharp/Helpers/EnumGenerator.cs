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
        // todo: Use ThreadLocal<IndentedStringBuilder> for re-use?
        var stringBuilder = new IndentedStringBuilder();
        _ = stringBuilder.Append("namespace ").AppendLine(nameSpace).AppendLine("{");

        using (_ = stringBuilder.Indent())
        {
            if (comment is not null)
                _ = stringBuilder.AppendLine(comment);

            if (isFlagEnum)
                _ = stringBuilder.AppendLine("[global::System.Flags]");

            _ = stringBuilder
                .Append(accessibility)
                .Append(" enum ")
                .AppendLine(name)
                .AppendLine("{");

            using (_ = stringBuilder.Indent())
            {
                if (isFlagEnum)
                    AppendFlagEnumValues(stringBuilder, enumValues);
                else
                    AppendEnumValues(stringBuilder, enumValues);
            }
            _ = stringBuilder.AppendLine("}");
        }
        _ = stringBuilder.AppendLine("}");

        return stringBuilder.ToString();
    }

    private static void AppendEnumValues(
        IndentedStringBuilder stringBuilder,
        IReadOnlyList<EnumValueDeclaration> values
    )
    {
        foreach (var (value, comment) in values)
        {
            if (comment is not null)
                _ = stringBuilder.AppendLine(comment);

            _ = stringBuilder.Append(value).AppendLine(",");
        }
    }

    private static void AppendFlagEnumValues(
        IndentedStringBuilder stringBuilder,
        IReadOnlyList<EnumValueDeclaration> values
    )
    {
        _ = stringBuilder.AppendLine("None = 0,");

        for (var i = 0; i < values.Count; i++)
        {
            var (value, comment) = values[i];

            if (comment is not null)
                _ = stringBuilder.AppendLine(comment);

            _ = stringBuilder.Append(value).Append(" = 1 << ").Append(i).AppendLine(",");
        }

        _ = stringBuilder.Append("All = (1 << ").Append(values.Count).AppendLine(") - 1");
    }
}
