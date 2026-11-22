using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpPluginLoader.HookGenerator;

public enum DiagnosticCode
{
    /// <summary>
    /// Method with HookAttribute not inside a class marked with HookProviderAttribute.
    /// </summary>
    HSG001,
    /// <summary>
    /// HookAttribute with neither Address nor Pattern property.
    /// </summary>
    HSG002,
    /// <summary>
    /// HookProviderAttribute class is not partial.
    /// </summary>
    HSG003,
}

public static class Diagnostics
{
    private static readonly Dictionary<DiagnosticCode, DiagnosticDescriptor> _diagnostics = new()
    {
        [DiagnosticCode.HSG001] = new DiagnosticDescriptor(
            DiagnosticCode.HSG001.ToString(),
            "HookGenerator",
            "This method is marked with HookAttribute but is not inside a class marked with HookProviderAttribute."
            "HookGenerator",
            DiagnosticSeverity.Error,
            true
        ),
        [DiagnosticCode.HSG002] = new DiagnosticDescriptor(
            DiagnosticCode.HSG002.ToString(),
            "HookGenerator",
            "HookAttribute must have either an Address or a Pattern property.",
            "HookGenerator",
            DiagnosticSeverity.Error,
            true
        ),
        [DiagnosticCode.HSG003] = new DiagnosticDescriptor(
            DiagnosticCode.HSG003.ToString(),
            "HookGenerator",
            "HookProviderAttribute class must be partial.",
            "HookGenerator",
            DiagnosticSeverity.Error,
            true
        ),
    };

    public static DiagnosticDescriptor GetDiagnostic(DiagnosticCode code) => _diagnostics[code];
}
