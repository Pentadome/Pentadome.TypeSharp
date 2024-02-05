using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pentadome.TypeSharp.Extensions;

internal static class SourceProductionContextExtensions
{
    public static void AddUniqueCsharpSource(
        this SourceProductionContext @this,
        string fileName,
        SourceText sourceText
    )
    {
        try
        {
            @this.AddSource($"{fileName}.g.cs", sourceText);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("must be unique"))
        {
            for (var i = 2; i < int.MaxValue; i++)
            {
                try
                {
                    @this.AddSource($"{fileName}{i}.g.cs", sourceText);
                    return;
                }
                catch (ArgumentException ex2) when (ex2.Message.Contains("must be unique"))
                { //NOOP
                }
            }
        }
    }
}
