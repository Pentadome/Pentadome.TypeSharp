﻿#pragma warning disable
#pragma warning restore TS1001
// Resharper disable All
using Pentadome.TypeSharp;
using Pentadome.TypeSharp.Sample;

// [assembly: TypeSharp.GenerateKeyMapOf(typeof(test2))]
[assembly: GenerateKeyMapOf<Test2>()]

[assembly: GenerateKeyMapOf<Test2>(Name = "Test2OrderBy")]

[assembly: GenerateKeyMapOf<Test2>(Name = "Test2OrderBy", NameSpace = "MyNameSpace")]

[assembly: GenerateKeyMapOf<Test2>(
    Name = "Test2WithInvalidExcludeArgKeyMap",
    Exclude = ["Something"]
)]

[assembly: GenerateKeyMapOf<Test2>(
    Name = "Test2WithoutAgeKeyMap",
    // testing CollectionExpressionSyntax
    Exclude = [nameof(Test2.Age)],
    // testing GeneratedKeyMapAccessibility.Target
    Accessibility = GeneratedKeyMapAccessibility.Target,
    // testing GeneratedKeyMapKind.Enum
    Kind = GeneratedKeyMapKind.Enum
)]

[assembly: GenerateKeyMapOf<Test2>(
    Name = "Test2WithoutAgeKeyMap2",
    // testing ImplicitArrayCreationExpressionSyntax
    Exclude = new[] { nameof(Test2.Age) },
    // testing GeneratedKeyMapAccessibility.Internal
    Accessibility = GeneratedKeyMapAccessibility.Internal,
    // testing GeneratedKeyMapKind.FlagEnum
    Kind = GeneratedKeyMapKind.FlagEnum
)]

[assembly: GenerateKeyMapOf<Test2>(
    Name = "Test2WithoutAgeKeyMap3",
    // testing ArrayCreationExpressionSyntax
    Exclude = new String[] { nameof(Test2.Age) },
    // testing GeneratedKeyMapAccessibility.Public
    Accessibility = GeneratedKeyMapAccessibility.Public
)]
