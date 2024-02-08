using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SharpPluginLoader.InternalCallGenerator;

[Generator]
public class InternalCallSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "InternalCallAttribute.g.cs",
            SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)
        ));
        
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

        var methodsToGenerate = GetMethodsToGenerate(compilation, distinctICalls, context.CancellationToken);

        if (methodsToGenerate.Count == 0)
            return;

        var className = methodsToGenerate.First().Method.ContainingType.Name;
        var source = SourceGenerationHelper.GenerateSource(new InternalCallCollection(className, methodsToGenerate), context);
        context.AddSource("InternalCalls.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    public static List<InternalCallMethod> GetMethodsToGenerate(Compilation compilation,
        IEnumerable<MethodDeclarationSyntax> methods, CancellationToken ct)
    {
        List<InternalCallMethod> methodsToGenerate = [];

        var icallAttribute = compilation.GetTypeByMetadataName(SourceGenerationHelper.FullAttributeName);
        if (icallAttribute is null)
            return methodsToGenerate;

        foreach (var method in methods)
        {
            ct.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(method.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(method) is not IMethodSymbol methodSymbol)
                continue;

            if (methodSymbol.IsGenericMethod || !methodSymbol.IsPartialDefinition) // Generic methods are not supported
                continue;
            
            var attributeData = methodSymbol.GetAttributes().FirstOrDefault(
                ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, icallAttribute));
            var isUnsafe = attributeData?.ConstructorArguments
                .Any(tc => tc.Type?.ToDisplayString() == SourceGenerationHelper.FullOptionsEnumName 
                           && tc.Value is not null
                           && (int)tc.Value == 1) ?? false;
            methodsToGenerate.Add(new InternalCallMethod(methodSymbol, isUnsafe));
        }

        return methodsToGenerate;
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

                if (fullName == SourceGenerationHelper.FullAttributeName)
                    return method;
            }
        }

        return null;
    }
}
