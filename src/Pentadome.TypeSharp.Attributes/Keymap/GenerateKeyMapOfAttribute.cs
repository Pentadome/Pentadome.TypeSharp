namespace Pentadome.TypeSharp;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class GenerateKeyMapOfAttribute : Attribute
{
    private string[]? _exclude;

    public string? Name { get; set; }

    public string? NameSpace { get; set; }

    public string[] Exclude
    {
        get => _exclude ??= [];
        set => _exclude = value;
    }

    public GeneratedKeyMapAccessibility Accessibility { get; set; }

    public GeneratedKeyMapKind Kind { get; set; }
}
