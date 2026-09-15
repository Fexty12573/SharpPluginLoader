using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpPluginLoader.HookGenerator;

[Generator]
public class HookSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var internalCalls = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)!)
            .Where(static m => m is not null);

        IncrementalValueProvider<(Compilation, ImmutableArray<MethodDeclarationSyntax>)> compilationAndInternalCalls
            = context.CompilationProvider.Combine(internalCalls.Collect());

        context.RegisterSourceOutput(compilationAndInternalCalls,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    public static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods,
        SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
            return;

        var distinctICalls = methods.Distinct();

        var methodsToGenerate = GetHooksToGenerate(compilation, distinctICalls, context, context.CancellationToken);

        if (methodsToGenerate.Count == 0)
            return;

        foreach (var hookCollection in methodsToGenerate.Values)
        {
            var source = SourceGenerationHelper.GenerateHookClass(hookCollection);
            context.AddSource($"{hookCollection.ContainingType.Name}_Hooks.g.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    public static Dictionary<INamedTypeSymbol, HookCollection> GetHooksToGenerate(Compilation compilation,
        IEnumerable<MethodDeclarationSyntax> methods, SourceProductionContext context, CancellationToken ct)
    {
        Dictionary<INamedTypeSymbol, HookCollection> hooksToGenerate = [];

        var hookAttribute = compilation.GetTypeByMetadataName(SourceGenerationHelper.HookAttributeName);
        if (hookAttribute is null)
            return hooksToGenerate;

        var hookProviderAttribute = compilation.GetTypeByMetadataName(SourceGenerationHelper.HookProviderAttributeName);
        if (hookProviderAttribute is null)
            return hooksToGenerate;

        foreach (var method in methods)
        {
            ct.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol)
                continue;

            // Skip generic methods and partial definitions
            if (methodSymbol.IsGenericMethod || methodSymbol.IsPartialDefinition)
                continue;

            var attributeData = methodSymbol.GetAttributes().FirstOrDefault(
                ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, hookAttribute));

            if (attributeData is null)
                continue;

            // Check for HookProviderAttribute
            var containingType = methodSymbol.ContainingType;
            if (containingType is null)
                continue;

            if (containingType.GetAttributes().FirstOrDefault(
                ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, hookProviderAttribute)) is null)
            {
                ReportDiagnostic(DiagnosticCode.HSG001, method.GetLocation(), context);
                continue;
            }

            // Check for named arguments
            long address = 0;
            string? pattern = null;
            var offset = 0;
            var cache = true;

            if (attributeData is not null)
            {
                foreach (var namedArg in attributeData.NamedArguments)
                {
                    switch (namedArg.Key)
                    {
                        case SourceGenerationHelper.AddressPropertyName:
                            address = (long)namedArg.Value.Value!;
                            break;
                        case SourceGenerationHelper.PatternPropertyName:
                            pattern = (string?)namedArg.Value.Value!;
                            break;
                        case SourceGenerationHelper.OffsetPropertyName:
                            offset = (int)namedArg.Value.Value!;
                            break;
                        case SourceGenerationHelper.CachePropertyName:
                            cache = (bool)namedArg.Value.Value!;
                            break;
                    }
                }
            }

            if (address == 0 && pattern is null)
            {
                ReportDiagnostic(DiagnosticCode.HSG002, method.GetLocation(), context);
                continue;
            }

            if (hooksToGenerate.TryGetValue(containingType, out var hookCollection))
            {
                hookCollection.Methods.Add(new HookMethod(methodSymbol, address, pattern, offset, cache));
            }
            else
            {
                hooksToGenerate[containingType] = new HookCollection(containingType)
                {
                    Methods = [new HookMethod(methodSymbol, address, pattern, offset, cache)]
                };
            }
        }

        return hooksToGenerate;
    }

    public static bool IsSyntaxTargetForGeneration(SyntaxNode syntax)
    {
        return syntax is MethodDeclarationSyntax { AttributeLists.Count: > 0 };
    }

    public static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        foreach (var attributeList in method.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                var attributeType = attributeSymbol.ContainingType;
                var fullName = attributeType?.ToDisplayString() ?? string.Empty;

                if (fullName == SourceGenerationHelper.HookAttributeName)
                    return method;
            }
        }
        
        return null;
    }

    private static void ReportDiagnostic(DiagnosticCode code, Location location, SourceProductionContext context)
    {
        var diagnostic = Diagnostic.Create(Diagnostics.GetDiagnostic(code), location);
        context.ReportDiagnostic(diagnostic);
    }
}
