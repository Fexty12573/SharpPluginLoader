using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpPluginLoader.HookGenerator;

public static class SourceGenerationHelper
{
    public const string HookAttributeName = "SharpPluginLoader.Core.Memory.HookAttribute";
    public const string HookProviderAttributeName = "SharpPluginLoader.Core.Memory.HookProviderAttribute";
    public const string AddressPropertyName = "Address";
    public const string PatternPropertyName = "Pattern";
    public const string OffsetPropertyName = "Offset";
    public const string CachePropertyName = "Cache";

    private static StringBuilder? _sb;
    private static int _indentLevel;
    public static string GenerateHookClass(HookCollection hooks)
    {
        if (hooks.Methods.Count == 0)
            return string.Empty;

        _sb = new StringBuilder();
        _indentLevel = 0;

        var containingType = hooks.ContainingType;
        var namespaceName = containingType.ContainingNamespace.ToDisplayString();
        var className = containingType.Name;

        AppendLine("#nullable enable");
        AppendLine("using System;");
        AppendLine("using System.Runtime.CompilerServices;");
        AppendLine("using System.Runtime.InteropServices;");
        AppendLine("using SharpPluginLoader.Core.Memory;");

        var classDefSb = new StringBuilder();

        classDefSb.Append(containingType.DeclaredAccessibility switch
        {
            Accessibility.Public => "public ",
            Accessibility.Internal => "internal ",
            Accessibility.Private => "private ",
            Accessibility.Protected => "protected ",
            Accessibility.ProtectedAndInternal => "protected internal ",
            Accessibility.ProtectedOrInternal => "protected internal ",
            _ => "internal "
        });

        if (containingType.IsStatic)
            classDefSb.Append("static ");

        classDefSb.Append("partial class ");

        classDefSb.Append(className);

        Append($$"""

            namespace {{namespaceName}};

            {{classDefSb}}
            {

            """);
        Indent();

        foreach (var method in hooks.Methods)
        {

        }

        return _sb.ToString();
    }

    private static void Append(string value)
    {
        _sb!.Append(_indentLevel == 0 ? value : new string(' ', _indentLevel * 4) + value);
    }

    private static void AppendLine(string value)
    {
        _sb!.AppendLine(_indentLevel == 0 ? value : new string(' ', _indentLevel * 4) + value);
    }

    private static void AppendLine()
    {
        _sb!.AppendLine();
    }

    private static void Indent()
    {
        _indentLevel++;
    }

    private static void Unindent()
    {
        _indentLevel--;
    }
}
