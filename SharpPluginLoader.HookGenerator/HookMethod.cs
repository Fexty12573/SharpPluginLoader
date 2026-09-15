using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SharpPluginLoader.HookGenerator;

public class HookCollection(INamedTypeSymbol containingType)
{
    public INamedTypeSymbol ContainingType = containingType;
    public List<HookMethod> Methods = [];
}

public readonly struct HookMethod(IMethodSymbol method, long address, string? pattern, int offset, bool cache)
{
    public IMethodSymbol Method { get; } = method;
    public long Address { get; } = address;
    public string? Pattern { get; } = pattern;
    public int Offset { get; } = offset;
    public bool Cache { get; } = cache;
}
