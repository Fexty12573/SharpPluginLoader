using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SharpPluginLoader.InternalCallGenerator;

using TransformedType = (string? typeName, TypeConversionKind conversion);

public static class SourceGenerationHelper
{
    public const string GeneratorNamespace = "SharpPluginLoader.InternalCallGenerator";
    public const string AttributeName = "InternalCallAttribute";
    public const string OptionsEnumName = "InternalCallOptions";
    public const string WideStringAttributeName = "WideStringAttribute";
    public const string FullAttributeName = $"{GeneratorNamespace}.{AttributeName}";
    public const string FullOptionsEnumName = $"{GeneratorNamespace}.{OptionsEnumName}";
    public const string FullWideStringAttributeName = $"{GeneratorNamespace}.{WideStringAttributeName}";
    public const string Attribute = $$"""
                                    namespace {{GeneratorNamespace}};
                                    
                                    public enum InternalCallOptions
                                    {
                                        None = 0,
                                        
                                        // Suppresses the GC transition when calling the function pointer
                                        Unsafe = 1
                                    }

                                    [System.AttributeUsage(System.AttributeTargets.Method)]
                                    public class {{AttributeName}}(InternalCallOptions options = InternalCallOptions.None) : System.Attribute
                                    {
                                        public InternalCallOptions {{OptionsPropertyName}} = options;
                                        public long {{AddressPropertyName}};
                                        public string? {{PatternPropertyName}};
                                        public int {{OffsetPropertyName}};
                                        public bool {{CachePropertyName}} = true;
                                    }
                                    
                                    [System.AttributeUsage(System.AttributeTargets.Parameter | System.AttributeTargets.ReturnValue)]
                                    public class {{WideStringAttributeName}} : System.Attribute;
                                    """;

    public const string OptionsPropertyName = "Options";
    public const string AddressPropertyName = "Address";
    public const string PatternPropertyName = "Pattern";
    public const string OffsetPropertyName = "Offset";
    public const string CachePropertyName = "Cache";

    private static IMethodSymbol? _currentMethodSymbol;

    public static string GenerateSource(List<InternalCallMethod> internalCalls, SourceProductionContext context)
    {
        if (internalCalls.Count == 0)
            return "";

        var sb = new StringBuilder();
        var fieldTypeSb = new StringBuilder(); // Used to generate the fields
        var methodSb = new StringBuilder(); // Used to generate the methods
        var methodBodySb = new StringBuilder(); // Used to generate the method body, cleared after each method
        var uploadMethodSb = new StringBuilder(); // Used to generate the upload method
        List<string> methodInvokeParams = []; // Used to store variable names for the function pointer invocation
        List<string> pinStatements = []; // Used to store variables that need to be pinned
        List<string> gcHandles = []; // Used to store the gc handles

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Text;");
        sb.AppendLine("using System.Runtime.InteropServices;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using SharpPluginLoader.Core;");
        sb.AppendLine("using SharpPluginLoader.Core.Memory;");
        sb.Append($$"""
                  
                  namespace {{internalCalls[0].Method.ContainingNamespace.ToDisplayString()}};

                  public static unsafe partial class InternalCalls
                  {
                  
                  """);

        // Generate start of upload method
        uploadMethodSb.Append("""
                              
                              public static void UploadInternalCalls(System.Collections.Generic.Dictionary<string, nint> icalls)
                              {
                              
                              """);

        // Each internal call needs
        // - A field of the form: private static delegate* unmanaged<T..., R> <MethodName>Ptr;
        // - A method of the form: public static unsafe partial R <MethodName>(T...);

        // The allowed parameter/return types are:
        // - Primitive types
        // - Pointer types
        // - Array types where the element type is a primitive type or a pointer type
        // - ref/out types
        // - string
        foreach (var (method, isUnsafe) in internalCalls)
        {
            _currentMethodSymbol = method;

            var transformedReturnType = TransformReturn(method, context);
            if (transformedReturnType.typeName is null)
                continue;

            // Start field generation
            fieldTypeSb.Append(isUnsafe
                ? "delegate* unmanaged[SuppressGCTransition]<"
                : "delegate* unmanaged<");

            var retModifier = "";
            if (method.ReturnsByRef)
                retModifier = "ref ";
            else if (method.ReturnsByRefReadonly)
                retModifier = "ref readonly ";
            
            var unsafeModifier = IsMethodDeclaredUnsafe(method) ? "unsafe " : "";
            
            // Start method generation
            methodSb.Append($"public static {unsafeModifier}partial {retModifier}{method.ReturnType.ToDisplayString()} {method.Name}(");

            if (!method.ReturnsVoid)
                methodBodySb.AppendLine($"{transformedReturnType.typeName} __result = default;");

            for (var i = 0; i < method.Parameters.Length; ++i)
            {
                var param = method.Parameters[i];
                var (typeName, conversion) = TransformParameter(param, context);

                if (typeName is null)
                    continue;
                
                var isLast = i == method.Parameters.Length - 1;

                // Add the type to the field
                fieldTypeSb.Append(typeName);
                fieldTypeSb.Append(", "); // Comma always added as the return type is last

                // Add the type to the method
                methodSb.Append(param.ToDisplayString());
                if (!isLast)
                    methodSb.Append(", ");

                // Convert the parameter
                string invokeParamName;
                switch (conversion)
                {
                    case TypeConversionKind.Array:
                        // Need to pin the array and get a pointer to the first element
                        invokeParamName = $"t__{param.Name}";
                        methodBodySb.AppendLine($"GCHandle gch_{param.Name} = GCHandle.Alloc({param.Name}, GCHandleType.Pinned);");
                        methodBodySb.AppendLine(
                            $"{typeName} {invokeParamName} = MemoryUtil.AsPointer(ref MemoryMarshal.GetArrayDataReference({param.Name}));");
                        methodInvokeParams.Add(invokeParamName);
                        gcHandles.Add($"gch_{param.Name}.Free();");
                        break;
                    case TypeConversionKind.RefOut:
                        switch (param.RefKind)
                        {
                            case RefKind.Ref:
                                invokeParamName = $"t__{param.Name}";
                                pinStatements.Add($"fixed({typeName} {invokeParamName} = &{param.Name})");
                                methodInvokeParams.Add(invokeParamName);
                                break;
                            case RefKind.Out:
                                methodBodySb.AppendLine($"Unsafe.SkipInit(out {param.Name});");
                                invokeParamName = $"t__{param.Name}";
                                pinStatements.Add($"fixed({typeName} {invokeParamName} = &{param.Name})");
                                methodInvokeParams.Add(invokeParamName);
                                break;
                            case RefKind.In:
                                ReportError(context, "ICG003", "In parameters are not supported");
                                methodInvokeParams.Add(param.Name);
                                break;
                            case RefKind.RefReadOnlyParameter:
                                ReportError(context, "ICG004", "Ref readonly parameters are not supported");
                                methodInvokeParams.Add(param.Name);
                                break;
                            case RefKind.None:
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        break;
                    case TypeConversionKind.String:
                        invokeParamName = $"t__{param.Name}";
                        methodBodySb.AppendLine($"byte[] b__{param.Name} = [..Encoding.UTF8.GetBytes({param.Name}), 0];");
                        pinStatements.Add($"fixed(byte* {invokeParamName} = b__{param.Name})");
                        methodInvokeParams.Add(invokeParamName);
                        break;
                    case TypeConversionKind.WideString:
                        invokeParamName = $"t__{param.Name}";
                        methodBodySb.AppendLine($"byte[] b__{param.Name} = [..Encoding.Unicode.GetBytes({param.Name}), 0, 0];");
                        pinStatements.Add($"fixed(byte* {invokeParamName} = b__{param.Name})");
                        methodInvokeParams.Add(invokeParamName);
                        break;
                    case TypeConversionKind.Span:
                        invokeParamName = $"t__{param.Name}";
                        pinStatements.Add($"fixed({typeName} {invokeParamName} = {param.Name})");
                        methodInvokeParams.Add(invokeParamName);
                        break;
                    case TypeConversionKind.Memory:
                        invokeParamName = $"({typeName})h__{param.Name}.Pointer";
                        methodBodySb.AppendLine($"using var h__{param.Name} = {param.Name}.Pin();");
                        methodInvokeParams.Add(invokeParamName);
                        break;
                    case TypeConversionKind.List:
                        invokeParamName = $"t__{param.Name}";
                        pinStatements.Add($"fixed({typeName} {invokeParamName} = CollectionsMarshal.AsSpan({param.Name}))");
                        methodInvokeParams.Add(invokeParamName);
                        break;
                    case TypeConversionKind.None:
                    default:
                        // No conversion needed
                        methodInvokeParams.Add(param.Name);
                        break;
                }
            }

            // Write the pin statements
            foreach (var pinStatement in pinStatements)
                methodBodySb.AppendLine(pinStatement);

            // Invoke the function pointer
            methodBodySb.AppendLine(transformedReturnType.typeName == "void"
                ? $"_{method.Name}Ptr({string.Join(", ", methodInvokeParams)});"
                : $"__result = _{method.Name}Ptr({string.Join(", ", methodInvokeParams)});");

            foreach (var gch in gcHandles)
                methodBodySb.AppendLine(gch);
            
            // Add the return statement
            if (transformedReturnType.typeName != "void")
            {
                switch (transformedReturnType.conversion)
                {
                    case TypeConversionKind.None:
                        methodBodySb.AppendLine("return __result;");
                        break;
                    case TypeConversionKind.Array:
                        throw new InvalidOperationException("Array return types are not supported");
                    case TypeConversionKind.RefOut:
                        methodBodySb.AppendLine("return ref *__result;");
                        break;
                    case TypeConversionKind.String:
                        methodBodySb.AppendLine("return MemoryUtil.ReadString(__result, Encoding.UTF8);");
                        break;
                    case TypeConversionKind.WideString:
                        methodBodySb.AppendLine("return MemoryUtil.ReadString(__result, Encoding.Unicode);");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // End field type generation
            fieldTypeSb.Append($"{transformedReturnType.typeName}>");

            // Generate field
            sb.AppendLine($"private static {fieldTypeSb} _{method.Name}Ptr;");

            // End method generation
            methodSb.AppendLine(")");
            methodSb.Append($$"""
                             {
                                 {{methodBodySb}}
                             }
                             """);

            // Generate method
            sb.AppendLine(methodSb.ToString());

            // Add entry to upload method
            var fieldType = fieldTypeSb.ToString();
            uploadMethodSb.AppendLine($"""
                                       if (icalls.TryGetValue("{method.Name}", out var {method.Name}Ptr))
                                           _{method.Name}Ptr = ({fieldType}){method.Name}Ptr;
                                       else
                                           Log.Warn("Could not find InternalCall for method {method.Name}");
                                       """);
            
            // Clear reused string builders and lists
            methodBodySb.Clear();
            methodInvokeParams.Clear();
            pinStatements.Clear();
            gcHandles.Clear();
            fieldTypeSb.Clear();
            methodSb.Clear();
        }

        // End upload method
        uploadMethodSb.AppendLine("}");
        
        // Generate upload method
        sb.AppendLine(uploadMethodSb.ToString());

        // End class
        sb.AppendLine("}");
        
        return sb.ToString();
    }

    private static bool IsMethodDeclaredUnsafe(IMethodSymbol method)
    {
        var syntax = method.DeclaringSyntaxReferences.First().GetSyntax() as MethodDeclarationSyntax;
        if (syntax is null)
            return false;

        foreach (var modifier in syntax.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.UnsafeKeyword))
                return true;
        }

        return false;
    }

    private static TransformedType TransformParameter(IParameterSymbol param, SourceProductionContext context)
    {
        switch (param.Type)
        {
            case IArrayTypeSymbol arrayType:
                if (param.RefKind != RefKind.None)
                {
                    ReportError(context, "ICG005", "Ref/out array parameters are not supported");
                    return (null, TypeConversionKind.None);
                }
                return (TransformType(arrayType.ElementType, context).typeName + "*", TypeConversionKind.Array);
            case IPointerTypeSymbol pointerType:
                var (typeName, _) = TransformType(pointerType.PointedAtType, context);
                return param.RefKind is RefKind.Ref or RefKind.Out 
                    ? (typeName + "**", TypeConversionKind.RefOut) // ref/out pointers are represented as pointer to pointer
                    : (typeName + "*", TypeConversionKind.None);
            case INamedTypeSymbol namedType:
                if (namedType.SpecialType == SpecialType.System_String)
                {
                    return HasWideStringAttribute(param)
                        ? ("byte*", TypeConversionKind.WideString)
                        : ("byte*", TypeConversionKind.String);
                }

                if (namedType.IsGenericType)
                {
                    switch (namedType.ConstructUnboundGenericType().ToDisplayString())
                    {
                        case "System.ReadOnlySpan<>":
                        case "System.Span<>":
                            var (spanType, _) = TransformType(namedType.TypeArguments[0], context);
                            return (spanType + "*", TypeConversionKind.Span);
                        case "System.ReadOnlyMemory<>":
                        case "System.Memory<>":
                            var (memoryType, _) = TransformType(namedType.TypeArguments[0], context);
                            return (memoryType + "*", TypeConversionKind.Memory);
                        case "System.Collections.Generic.List<>":
                            var (listType, _) = TransformType(namedType.TypeArguments[0], context);
                            return (listType + "*", TypeConversionKind.List);
                    }
                }

                if (!namedType.IsValueType)
                {
                    ReportError(context, "ICG001A", "Reference types are not supported");
                    return (null, TypeConversionKind.None);
                }

                var (type, _) = TransformType(namedType, context);
                return param.RefKind is RefKind.Ref or RefKind.Out 
                    ? (type + "*", TypeConversionKind.RefOut) // ref/out value types are represented as pointer to value type
                    : (type, TypeConversionKind.None);
            case ITypeParameterSymbol:
                ReportError(context, "ICG002", "Type parameters are not supported");
                return (null, TypeConversionKind.None);
            default:
                return (param.Type.ToDisplayString(), TypeConversionKind.None);
        }
    }

    private static TransformedType TransformReturn(IMethodSymbol method, SourceProductionContext context)
    {
        if (method.ReturnsVoid)
            return ("void", TypeConversionKind.None);

        switch (method.ReturnType)
        {
            case IArrayTypeSymbol:
                ReportError(context, "ICG006", "Array return types are not supported. Use a pointer instead.");
                return (null, TypeConversionKind.None);
            case IPointerTypeSymbol pointerType:
                var (typeName, _) = TransformType(pointerType.PointedAtType, context);
                return method.ReturnsByRef 
                    ? (typeName + "**", TypeConversionKind.RefOut) 
                    : (typeName + "*", TypeConversionKind.None);
            case INamedTypeSymbol namedType:
                if (namedType.SpecialType == SpecialType.System_String)
                {
                    return HasWideStringAttribute(method) 
                        ? ("byte*", TypeConversionKind.WideString) 
                        : ("byte*", TypeConversionKind.String);
                }
                
                if (namedType.IsGenericType || !namedType.IsValueType)
                {
                    ReportError(context, "ICG001B", "Generic and reference return types are not supported");
                    return (null, TypeConversionKind.None);
                }
                
                var (type, _) = TransformType(namedType, context);
                return method.ReturnsByRef 
                    ? (type + "*", TypeConversionKind.RefOut) 
                    : (type, TypeConversionKind.None);
            case ITypeParameterSymbol:
                ReportError(context, "ICG002", "Type parameters are not supported");
                return (null, TypeConversionKind.None);
            default:
                return (method.ReturnType.ToDisplayString(), TypeConversionKind.None);
        }
    }

    private static TransformedType TransformType(ITypeSymbol type, SourceProductionContext context)
    {
        switch (type)
        {
            case IArrayTypeSymbol arrayType:
                return (TransformType(arrayType.ElementType, context).typeName + "*", TypeConversionKind.Array);
            case IPointerTypeSymbol pointerType:
                return (TransformType(pointerType.PointedAtType, context).typeName + "*", TypeConversionKind.None);
            case INamedTypeSymbol namedType:
                if (namedType.IsGenericType || !namedType.IsValueType)
                {
                    ReportError(context, "ICG001", "Generic types and reference types are not supported");
                    return (null, TypeConversionKind.None);
                }
                
                return namedType.IsRefLikeType
                    ? ($"{namedType.ToDisplayString()}*", TypeConversionKind.RefOut)
                    : (namedType.ToDisplayString(), TypeConversionKind.None);
            case ITypeParameterSymbol:
                ReportError(context, "ICG002", "Type parameters are not supported");
                return (null, TypeConversionKind.None);
            default:
                return (type.ToDisplayString(), TypeConversionKind.None);
        }
    }

    private static bool HasWideStringAttribute(IParameterSymbol parameter)
    {
        foreach (var attribute in parameter.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == FullWideStringAttributeName)
                return true;
        }

        return false;
    }

    private static bool HasWideStringAttribute(IMethodSymbol method)
    {
        foreach (var attribute in method.GetReturnTypeAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == FullWideStringAttributeName)
                return true;
        }

        return false;
    }

    private static void ReportError(SourceProductionContext context, string id, string message, IMethodSymbol? method = null)
    {
        context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    id,
                    "InternalCallGenerator",
                    message,
                    "InternalCallGenerator",
                    DiagnosticSeverity.Error,
                    true
                ),
                (method ?? _currentMethodSymbol)!.Locations[0]
            )
        );
    }
}

internal enum TypeConversionKind
{
    None = 0,
    Array = 1,
    RefOut = 2,
    String = 3,
    WideString = 4,
    Span = 5,
    List = 6,
    Memory = 7
}
