using Microsoft.CodeAnalysis;

namespace SharpPluginLoader.InternalCallGenerator;

public readonly struct InternalCallMethod(IMethodSymbol method, bool isUnsafe, long address, 
    string? pattern, int offset, bool cache)
{
    public IMethodSymbol Method { get; } = method;
    public bool IsUnsafe { get; } = isUnsafe;
    public long Address { get; } = address;
    public string? Pattern { get; } = pattern;
    public int Offset { get; } = offset;
    public bool Cache { get; } = cache;

    public void Deconstruct(out IMethodSymbol outMethod, out bool outIsUnsafe)
    {
        outMethod = Method;
        outIsUnsafe = IsUnsafe;
    }
}
