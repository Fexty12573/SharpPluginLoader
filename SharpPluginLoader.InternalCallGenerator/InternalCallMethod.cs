using Microsoft.CodeAnalysis;

namespace SharpPluginLoader.InternalCallGenerator;

public struct InternalCallMethod(IMethodSymbol method, bool isUnsafe)
{
    public IMethodSymbol Method = method;
    public bool IsUnsafe = isUnsafe;

    public readonly void Deconstruct(out IMethodSymbol outMethod, out bool outIsUnsafe)
    {
        outMethod = Method;
        outIsUnsafe = IsUnsafe;
    }
}
