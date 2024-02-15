using Microsoft.CodeAnalysis;
using Pentadome.TypeSharp.Attributes.Keymap;

namespace Pentadome.TypeSharp;

[Generator]
public class TypeSharpIncrementalSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG && FALSE
        // Use the Jit debugger for source generators.
        // Rider: https://blog.jetbrains.com/dotnet/2019/04/16/edit-continue-just-time-debugging-debugger-improvements-rider-2019-1/#just-in-time-debugger
        // Visual Studio: https://learn.microsoft.com/en-us/visualstudio/debugger/debug-using-the-just-in-time-debugger?view=vs-2022
        Debugger.Launch();
#endif
        GenerateKeyMapOfT.Initialize(context);
    }
}
