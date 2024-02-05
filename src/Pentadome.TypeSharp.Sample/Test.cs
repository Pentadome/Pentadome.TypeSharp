using Pentadome.TypeSharp;
using Pentadome.TypeSharp.Sample;

// [assembly: TypeSharp.GenerateKeyMapOf(typeof(test2))]
[assembly: GenerateKeyMapOf<Test2>()]

[assembly: GenerateKeyMapOf<Test2>(Name = "Test2OrderBy")]

[assembly: GenerateKeyMapOf<Test2>(Name = "Test2OrderBy", NameSpace = "MyNameSpace")]

[assembly: GenerateKeyMapOf<Test2>(Name = "Test2WithoutAgeKeyMa", Exclude = [nameof(Test2.Age)])]
