using Microsoft.CodeAnalysis;

namespace Pentadome.TypeSharp.Models;

internal readonly record struct ArrayValue<T>(T Value, Location Location);
