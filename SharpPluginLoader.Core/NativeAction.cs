using System;
using System.Linq;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Represents a function pointer to a native function with no return type and no arguments. Equivalent to <see cref="NativeFunction"/>.
    /// </summary>
    public class NativeAction
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
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
    public class NativeAction<T1>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, void> Invoke => (delegate* unmanaged<T1, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, void> Invoke => (delegate* unmanaged<T1, T2, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, void> Invoke => (delegate* unmanaged<T1, T2, T3, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void>)_funcPtr;
    }

    /// <summary>
    /// Represents a function pointer to a native function with the given return type and arguments.
    /// </summary>
    public class NativeAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        private readonly unsafe void* _funcPtr;

        public unsafe NativeAction(nint funcPtr)
        {
            _funcPtr = (void*)funcPtr;
        }

        public unsafe NativeAction(long funcPtr)
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
        public unsafe delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void> Invoke => (delegate* unmanaged<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>)_funcPtr;

        /// <summary>
        /// Invokes the function pointer without transitioning the Garbage Collector. Use this for very short functions.
        /// </summary>
        /// <remarks> See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute">SuppressGCTransitionAttribute</see></remarks>
        public unsafe delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void> InvokeUnsafe => (delegate* unmanaged[SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>)_funcPtr;
    }
}
