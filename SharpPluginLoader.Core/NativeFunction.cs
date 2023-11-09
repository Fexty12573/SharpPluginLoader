using System;
using System.Linq;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Represents a function pointer to a native function with no return type and no arguments. Equivalent to <see cref="NativeAction"/>.
    /// </summary>
    public class NativeFunction
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe void Invoke() => ((delegate* unmanaged<void>)_funcPtr)();

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute"/></remarks>
        public unsafe void InvokeUnsafe() => ((delegate* unmanaged[SuppressGCTransition]<void>)_funcPtr)();
    }
    
    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<TRet> Invoke => (delegate* unmanaged<TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, TRet> Invoke => (delegate* unmanaged<T1, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, TRet> Invoke => (delegate* unmanaged<T1, T2, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TRet>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRet>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeFunction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeFunction(long funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        /// <summary>
        /// Gets the native function pointer.
        /// </summary>
        public unsafe nint NativePointer => (nint)_funcPtr;

        /// <summary>
        /// Invokes the function pointer.
        /// </summary>
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRet> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRet>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRet> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TRet>)_funcPtr;
    }
}
