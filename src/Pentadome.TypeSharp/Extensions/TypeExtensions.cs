namespace Pentadome.TypeSharp.Extensions;

internal static class TypeExtensions
{
    private static readonly char[] _genericSuffixes =
    [
        '`',
        '0',
        '1',
        '2',
        '3',
        '4',
        '5',
        '6',
        '7',
        '8',
        '9'
    ];

    public static string ToDisplayString(this Type @this)
    {
        if (!@this.IsGenericTypeDefinition)
            return @this.FullName ?? throw new InvalidOperationException("Could not get name");

        // System.Collections.Generic.List`1
        var nameWithGenericTypeCount =
            @this.FullName ?? throw new InvalidOperationException("Could not get name");

        var nameWithoutGenericTypeCount = nameWithGenericTypeCount.TrimEnd(_genericSuffixes);

        var genericTypeNames = @this.GetGenericArguments().Select(x => x.Name);

        return $"{nameWithoutGenericTypeCount}<{string.Join(", ", genericTypeNames)}>";
    }
}
