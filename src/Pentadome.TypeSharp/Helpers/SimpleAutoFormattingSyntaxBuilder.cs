using System.Text;
using CSharpier;

namespace Pentadome.TypeSharp.Helpers;

internal sealed class SimpleAutoFormattingSyntaxBuilder
{
    private readonly StringBuilder _stringBuilder = new("// Formatted by CSharpier\n\n\n");

    public SimpleAutoFormattingSyntaxBuilder NameSpaceBracketOpenLine(string nameSpaceName)
    {
        _ = _stringBuilder.Append("namespace ").Append(nameSpaceName).AppendLine("{");
        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder ClassBracketOpenLine(
        string className,
        string accessibility,
        IReadOnlyCollection<string>? implementations = null
    )
    {
        _ = _stringBuilder.Append(accessibility).Append(" class ").Append(className);

        if (implementations?.Count > 0)
        {
            _ = _stringBuilder.Append(" : ");
            var isFirst = true;
            foreach (var implementation in implementations)
            {
                if (!isFirst)
                    _ = _stringBuilder.Append(", ");

                _ = _stringBuilder.Append(implementation);
                isFirst = false;
            }
        }

        _ = _stringBuilder.AppendLine("{");
        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder EnumBracketOpenLine(
        string enumName,
        string accessibility,
        bool prependFlagsAttribute = false
    )
    {
        if (prependFlagsAttribute)
            _ = _stringBuilder.AppendLine("[global::System.Flags]");

        _ = _stringBuilder.Append(accessibility).Append(" enum ").Append(enumName).AppendLine("{");

        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder XmlDocLine(string? summary)
    {
        if (summary is null)
            return this;

        summary = summary.Replace("\n", "\n/// ");

        _ = _stringBuilder
            .AppendLine("/// <summary>")
            .Append("/// ")
            .AppendLine(summary)
            .AppendLine("/// </summary>");

        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder Append(string text)
    {
        _ = _stringBuilder.Append(text);
        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder Append(int number)
    {
        _ = _stringBuilder.Append(number);
        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder AppendLine(string text)
    {
        _ = _stringBuilder.AppendLine(text);
        return this;
    }

    public override string ToString()
    {
        var unformatted = _stringBuilder.ToString();

        return CodeFormatter.Format(unformatted).Code;
    }

    public SimpleAutoFormattingSyntaxBuilder Clear()
    {
        _ = _stringBuilder.Clear();
        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder BracketOpenLine()
    {
        _ = _stringBuilder.AppendLine("{");
        return this;
    }

    public SimpleAutoFormattingSyntaxBuilder BracketCloseLine()
    {
        _ = _stringBuilder.AppendLine("}");
        return this;
    }
}
