using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace SharpPluginLoader.Core.Experimental
{
    internal static class DelegateFactory
    {
        public static TypedDelegate CreateDelegate(ModuleBuilder moduleBuilder, MethodInfo method, object? target = null, string? delegateTypeName = null)
        {
            delegateTypeName ??= $"{method.DeclaringType?.Name}_{method.Name}_Delegate";
            var delegateType = GetDelegateType(method, moduleBuilder, delegateTypeName);

            if (!method.IsStatic && target == null)
                throw new ArgumentNullException(nameof(target), "Target cannot be null for non-static methods.");

            var del = method.IsStatic ? 
                Delegate.CreateDelegate(delegateType, method) : 
                Delegate.CreateDelegate(delegateType, target, method);

            return new TypedDelegate(del, delegateType);
        }

        public static TypedDelegate CreateDelegate(Type delegateType, MethodInfo method, object? target = null)
        {
            if (!method.IsStatic && target == null)
                throw new ArgumentNullException(nameof(target), "Target cannot be null for non-static methods.");

            var del = method.IsStatic ?
                Delegate.CreateDelegate(delegateType, method) :
                Delegate.CreateDelegate(delegateType, target, method);

            return new TypedDelegate(del, delegateType);
        }

        private static Type GetDelegateType(MethodInfo method, ModuleBuilder moduleBuilder, string typeName)
        {
            // Define a new type that inherits from MulticastDelegate
            var typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
                typeof(MulticastDelegate));

            // Define the constructor
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new Type[] { typeof(object), typeof(IntPtr) });
            constructorBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            // Define the Invoke method
            var methodBuilder = typeBuilder.DefineMethod(
                "Invoke",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                method.ReturnType,
                method.GetParameters().Select(p => p.ParameterType).ToArray());
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            // Create the type
            return typeBuilder.CreateType();
        }
    }
}
