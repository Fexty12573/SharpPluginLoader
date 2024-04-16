using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyModel;

namespace SharpPluginLoader.Core.Scripting;

internal static class ScriptCompiler
{
    public static byte[]? Compile(string scriptPath)
    {
        var source = File.ReadAllText(scriptPath);
        
        using var bytecodeStream = new MemoryStream();

        var compilation = Compile(source, Path.GetFileNameWithoutExtension(scriptPath));
        var result = compilation.Emit(bytecodeStream);

        if (!result.Success)
        {
            Log.Error($"Failed to compile {scriptPath}:");
        }

        foreach (var diag in result.Diagnostics)
        {
            switch (diag.Severity)
            {
                case DiagnosticSeverity.Error:
                    Log.Error(diag.GetMessage());
                    break;
                case DiagnosticSeverity.Warning:
                    Log.Warn(diag.GetMessage());
                    break;
                case DiagnosticSeverity.Info:
                    Log.Info(diag.GetMessage());
                    break;
                case DiagnosticSeverity.Hidden:
                    Log.Debug(diag.GetMessage());
                    break;
            }
        }

        return result.Success ? bytecodeStream.ToArray() : null;
    }

    private static CSharpCompilation Compile(string source, string name)
    {
        var sourceText = SourceText.From(source);
        var compileOptions = CompilationOptions.WithScriptClassName(name);
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, ParseOptions);
        
        var references = GetDefaultReferences();
        return CSharpCompilation.Create(
            name + ".dll",
            [ syntaxTree ],
            references,
            compileOptions
        );
    }

    private static PortableExecutableReference[] GetDefaultReferences()
    {
        return DependencyContext.Default?.CompileLibraries
            .SelectMany(lib => lib.ResolveReferencePaths())
            .Select(AssemblyMetadata.CreateFromFile)
            .Select(meta => meta.GetReference())
            .ToArray() ?? [];
    }

    private static CSharpParseOptions ParseOptions { get; } = new CSharpParseOptions()
        .WithKind(SourceCodeKind.Script)
        .WithLanguageVersion(LanguageVersion.CSharp12);

    private static CSharpCompilationOptions CompilationOptions { get; } = 
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithOptimizationLevel(OptimizationLevel.Release)
        .WithPlatform(Platform.X64)
        .WithConcurrentBuild(true)
        .WithDeterministic(true)
        .WithGeneralDiagnosticOption(ReportDiagnostic.Default)
        .WithAllowUnsafe(true)
        .WithUsings(
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Text",
            "System.Threading.Tasks",
            "SharpPluginLoader.Core",
            "SharpPluginLoader.Core.Memory"
        );
}
