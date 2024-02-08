using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SharpPluginLoader.InternalCallGenerator;


public class InternalCallCollection(string className, List<InternalCallMethod> methods)
{
    public string ClassName = className;
    public List<InternalCallMethod> Methods = methods;

    public void Deconstruct(out string outClassName, out List<InternalCallMethod> outMethods)
    {
        outClassName = ClassName;
        outMethods = Methods;
    }
}

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
