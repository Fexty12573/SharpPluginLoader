using Microsoft.CodeAnalysis;
using System;

namespace SharpPluginLoader.HookGenerator;

[Generator]
public class HookSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        throw new NotImplementedException();
    }
}
