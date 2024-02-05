using JetBrains.Annotations;

namespace Pentadome.TypeSharp;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class GenerateKeyMapOfAttribute<[UsedImplicitly] T> : Attribute
{
    private string[]? _exclude;

    public string? Name { get; set; }

    public string? NameSpace { get; set; }

    public string[] Exclude
    {
        get => _exclude ??= [];
        set => _exclude = value;
    }

    public GenerateKeyMapOfAttribute.GeneratedKeyMapAccessibility Accessibility { get; set; }
}
