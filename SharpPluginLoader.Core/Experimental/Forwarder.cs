using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace SharpPluginLoader.Core.Experimental
{
    internal struct TypedDelegate
    {
        public Delegate Delegate;
        public Type Type;

        public TypedDelegate(Delegate @delegate, Type type)
        {
            Delegate = @delegate;
            Type = type;
        }
    }

    internal partial class Forwarder
    {
        public Delegate Original;
        public TypedDelegate Delegate;

        public Forwarder(Delegate original, ForwarderType type, Delegate? source = null)
        {
            Original = original;

            Delegate = type switch
            {
                ForwarderType.NativeToManaged => Create(original),
                ForwarderType.ManagedToNative => CreateReverse(original, source!),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }

    internal enum ForwarderType
    {
        NativeToManaged,
        ManagedToNative
    }

    internal partial class Forwarder
    {
        /// <summary>
        /// Creates a forwarder that forwards arguments from their native types to their managed types,
        /// as dictated by the parameters of <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The managed delegate that will be called</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private static TypedDelegate Create(Delegate original)
        {
            var originalType = original.GetType();
            var originalMethod = originalType.GetMethod("Invoke");
            if (originalMethod == null)
                throw new InvalidOperationException("Original delegate does not have an Invoke method");

            CheckMethodValidity(originalMethod);

            var originalParameters = originalMethod.GetParameters();
            var originalReturnType = originalMethod.ReturnType;

            var (forwarderReturnType, forwarderParamTypes)
                = GetForwarderTypes(originalReturnType, originalParameters, originalMethod);

            // If the original delegate does not need marshalling, we do not need to create a forwarder
            if (!originalParameters.Any(NeedsMarshalling) && !NeedsMarshalling(originalReturnType, originalMethod))
                return new TypedDelegate(original, originalType);

            var builder = ModuleBuilder.DefineType("Forwarder_" + Guid.NewGuid(),
                TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class, typeof(object));

            // Assign the `original` to this field after creating the delegate object
            var fieldBuilder = builder.DefineField("_original", originalType, FieldAttributes.Public);

            var invokeBuilder = builder.DefineMethod("Invoke",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                CallingConventions.HasThis,
                forwarderReturnType,
                forwarderParamTypes
            );
            invokeBuilder.SetImplementationFlags(MethodImplAttributes.IL);

            var ilGen = invokeBuilder.GetILGenerator();

            // Load the original delegate, since `Invoke` is an instance method
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, fieldBuilder);

            // Load all parameters
            // We always load param.Position+1 because the first parameter (of the dynamic Invoke)
            // is the dynamic delegate itself. The `Position` property of the parameter is 0-indexed,
            // regardless of if the method is static or not
            foreach (var param in originalParameters)
            {
                if (NeedsMarshalling(param))
                {
                    var paramType = param.ParameterType;
                    if (paramType.IsSubclassOf(typeof(NativeWrapper)))
                    {
                        var constructor = paramType.GetConstructor(new[] { typeof(nint) });
                        if (constructor == null)
                            throw new InvalidOperationException($"{paramType.Name} does not have a constructor that takes a nint");

                        // Load the native pointer from the parameter, and create a new instance of the wrapper
                        // using the constructor that takes a nint. After the call to the constructor, the
                        // wrapper is on the top of the stack.
                        ilGen.Emit(OpCodes.Ldarg, param.Position + 1);
                        ilGen.Emit(OpCodes.Newobj, constructor);
                    }
                    else if (HasCustomMarshallingAttribute(param))
                    {
                        // TODO: Implement custom marshalling
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarg, param.Position + 1);
                }
            }

            // Call the original delegate
            ilGen.Emit(OpCodes.Callvirt, originalMethod);

            // If the return type needs marshalling, we need to marshal it
            if (originalReturnType.IsSubclassOf(typeof(NativeWrapper)))
            {
                var property = originalReturnType.GetProperty("Instance");
                if (property == null)
                    throw new InvalidOperationException($"{originalReturnType.Name} does not have a property named Instance");

                var getter = property.GetGetMethod();
                if (getter == null)
                    throw new InvalidOperationException($"{originalReturnType.Name} does not have a getter for the Instance property");

                // At this point the wrapper is on the top of the stack, so we need to
                // get the native pointer from it and return that.
                ilGen.Emit(OpCodes.Callvirt, getter);
            }
            else if (HasCustomMarshallingAttribute(originalMethod))
            {
                throw new NotImplementedException();
            }

            // Return
            ilGen.Emit(OpCodes.Ret);


            // Create the type
            var type = builder.CreateType();
            if (type == null)
                throw new InvalidOperationException("Failed to create forwarder type");

            // Create an instance of the type
            var instance = Activator.CreateInstance(type);
            if (instance == null)
                throw new InvalidOperationException("Failed to create forwarder instance");

            // Set the original delegate so our forwarder can call it
            type.GetField("_original")!.SetValue(instance, original);

            return DelegateFactory.CreateDelegate(ModuleBuilder, type.GetMethod("Invoke")!, instance);
        }

        /// <summary>
        /// Creates a forwarder that forwards arguments from their managed types to their native types,
        /// as dictated by the parameters of <paramref name="outMethod"/>.
        /// </summary>
        /// <param name="outMethod">The delegate that wraps the native function to be called</param>
        /// <param name="inMethod">The delegate that calls this wrapper</param>
        /// <exception cref="InvalidOperationException"></exception>
        private static TypedDelegate CreateReverse(Delegate outMethod, Delegate inMethod)
        {
            // outMethod is a delegate that wraps the native function, and has native types.
            // inMethod is the delegate passed in to the native to managed forwarder, and has managed types.
            // We need to get the in-method, so we can determine which native types need to be marshalled to managed types.

            var originalType = outMethod.GetType();
            var originalMethod = originalType.GetMethod("Invoke");
            if (originalMethod == null)
                throw new InvalidOperationException("out-delegate does not have an Invoke method");

            CheckMethodValidity(originalMethod);

            var outParameters = originalMethod.GetParameters();
            var outParamTypes = outParameters.Select(p => p.ParameterType).ToArray();
            var outReturnType = originalMethod.ReturnType;

            var managedMethod = inMethod.GetType().GetMethod("Invoke");
            if (managedMethod == null)
                throw new InvalidOperationException("in-method does not have an Invoke method");
            var inParameters = inMethod.Method.GetParameters();
            var inParamTypes = inParameters.Select(p => p.ParameterType).ToArray();
            var inReturnType = inMethod.Method.ReturnType;

            if (!inParameters.Any(NeedsMarshalling) && !NeedsMarshalling(inReturnType, managedMethod))
                return new TypedDelegate(outMethod, originalType);

            var builder = ModuleBuilder.DefineType("Forwarder_" + Guid.NewGuid(),
                               TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class, typeof(object));

            // Assign the `original` to this field after creating the delegate object
            var fieldBuilder = builder.DefineField("_nativeFunc", originalType, FieldAttributes.Public);

            var invokeBuilder = builder.DefineMethod("Invoke",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot |
                MethodAttributes.Virtual,
                CallingConventions.HasThis,
                inReturnType,
                inParamTypes
            );
            invokeBuilder.SetImplementationFlags(MethodImplAttributes.IL);

            var ilGen = invokeBuilder.GetILGenerator();

            // Load the native delegate, since `Invoke` is an instance method
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, fieldBuilder);

            // Load all parameters
            // We always load param.Position+1 because the first parameter (of the dynamic Invoke)
            // is the dynamic delegate itself. The `Position` property of the parameter is 0-indexed,
            // regardless of if the method is static or not
            foreach (var param in inParameters)
            {
                if (NeedsMarshalling(param))
                {
                    var paramType = param.ParameterType;
                    if (paramType.IsSubclassOf(typeof(NativeWrapper)))
                    {
                        // Load the managed wrapper onto the stack. After calling `get_Instance`, the
                        // native pointer is on the top of the stack and we can continue loading the parameters.
                        ilGen.Emit(OpCodes.Ldarg, param.Position + 1);
                        ilGen.Emit(OpCodes.Callvirt, paramType.GetProperty("Instance")!.GetGetMethod()!);
                    }
                    else if (HasCustomMarshallingAttribute(param))
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    ilGen.Emit(OpCodes.Ldarg, param.Position + 1);
                }
            }

            // Call the native delegate
            ilGen.Emit(OpCodes.Callvirt, originalMethod);

            // If the return type needs marshalling, we need to marshal it
            if (inReturnType.IsSubclassOf(typeof(NativeWrapper)))
            {
                var constructor = inReturnType.GetConstructor(new[] { typeof(nint) });
                if (constructor == null)
                    throw new InvalidOperationException($"{inReturnType.Name} does not have a constructor that takes a nint");

                // At this point the native pointer is on the top of the stack, so we need to
                // create a new instance of the wrapper using the constructor that takes a nint.
                // After the call to the constructor, the wrapper is on the top of the stack.
                ilGen.Emit(OpCodes.Newobj, constructor);
            }
            else if (HasCustomMarshallingAttribute(originalMethod))
            {
                throw new NotImplementedException();
            }

            // Return
            ilGen.Emit(OpCodes.Ret);


            // Create the type
            var type = builder.CreateType();

            // Create an instance of the type
            var instance = Activator.CreateInstance(type);
            if (instance == null)
                throw new InvalidOperationException("Failed to create forwarder instance");

            // Set the original delegate so our forwarder can call it
            type.GetField("_nativeFunc")!.SetValue(instance, outMethod);

            return DelegateFactory.CreateDelegate(inMethod.GetType(), type.GetMethod("Invoke")!, instance);
        }


        private static void CheckMethodValidity(MethodInfo method)
        {
            // Check generics
            if (method.ContainsGenericParameters)
                throw new InvalidOperationException("Hook cannot have generic parameters");
            if (method.IsGenericMethod)
                throw new InvalidOperationException("Hook cannot be a generic method");
            if (method.IsGenericMethodDefinition)
                throw new InvalidOperationException("Hook cannot be a generic method definition");

            // Check parameters
            if (method.ReturnType == typeof(string))
                throw new InvalidOperationException("Hook cannot have a return type of string");
        }

        private static bool NeedsMarshalling(ParameterInfo param)
        {
            // Parameters need marshalling if:
            // a) The parameter inherits from NativeWrapper
            // b) The parameter has a custom marshalling attribute
            if (param.ParameterType.IsSubclassOf(typeof(NativeWrapper)))
                return true;
            // TODO: Check for custom marshalling attribute

            return false;
        }

        private static bool HasCustomMarshallingAttribute(ParameterInfo param)
        {
            // TODO: Change the name of the attribute to something more appropriate
            return param.GetCustomAttributes().Any(a => a.GetType().Name == "MarshalAsAttribute");
        }

        private static bool HasCustomMarshallingAttribute(MethodInfo method)
        {
            return method.GetCustomAttributes().Any(a => a.GetType().Name == "MarshalAsAttribute");
        }

        private static (Type, Type[]) GetForwarderTypes(Type originalReturn, IEnumerable<ParameterInfo> originalParams, MethodInfo method)
        {
            var forwarderParams = originalParams.Select(GetForwarderType).ToArray();
            return (GetForwarderType(originalReturn, method), forwarderParams);
        }

        private static Type GetForwarderType(ParameterInfo param)
        {
            if (param.ParameterType.IsSubclassOf(typeof(NativeWrapper)))
                return typeof(nint);

            // TODO: Check for custom marshalling attribute

            return param.ParameterType;
        }

        private static Type GetForwarderType(Type t, MethodInfo method)
        {
            if (t.IsSubclassOf(typeof(NativeWrapper)))
                return typeof(nint);

            // TODO: Check for custom marshalling attribute

            return t;
        }

        private static bool NeedsMarshalling(Type returnType, MethodInfo method)
        {
            // Return types need marshalling if:
            // a) The return type inherits from NativeWrapper
            // b) The method has a custom marshalling attribute
            if (returnType.IsSubclassOf(typeof(NativeWrapper)))
                return true;
            // TODO: Check for custom marshalling attribute
            return false;
        }

        static Forwarder()
        {
            var assemblyName = new AssemblyName("SharpPluginLoader.Core.Experimental.DynamicDelegates");
            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule("DynamicDelegateModule");
        }

        private static readonly AssemblyBuilder AssemblyBuilder;
        private static readonly ModuleBuilder ModuleBuilder;
    }
}
