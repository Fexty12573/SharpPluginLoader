using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// A wrapper around a native pointer.
    /// </summary>
    public class NativeWrapper
    {
        /// <summary>
        /// The native pointer.
        /// </summary>
        public nint Instance { get; internal init; }

        public NativeWrapper(nint instance)
        {
            Instance = instance;
        }

        public NativeWrapper()
        {
            Instance = 0;
        }

        /// <summary>
        /// Gets a value at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the value, must be an unmanaged type</typeparam>
        /// <param name="offset">The offset at which to retrieve the value</param>
        /// <returns>The value at the offset</returns>
        public unsafe T Get<T>(nint offset) where T : unmanaged
        {
            return *(T*)(Instance + offset);
        }

        /// <summary>
        /// Sets a value at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the value, must be an unmanaged type</typeparam>
        /// <param name="offset">The offset at which to set the value</param>
        /// <param name="value">The value to set</param>
        public unsafe void Set<T>(nint offset, T value) where T : unmanaged
        {
            *(T*)(Instance + offset) = value;
        }

        /// <inheritdoc cref="Get{T}(nint)"/>
        public unsafe T* GetPtr<T>(nint offset) where T : unmanaged
        {
            return *(T**)(Instance + offset);
        }

        /// <inheritdoc cref="Set{T}(nint,T)"/>
        public unsafe void SetPtr<T>(nint offset, T* value) where T : unmanaged
        {
            *(T**)(Instance + offset) = value;
        }

        /// <inheritdoc cref="Get{T}(nint)"/>
        public unsafe ref T GetRef<T>(nint offset) where T : unmanaged
        {
            return ref *(T*)(Instance + offset);
        }

#pragma warning disable CS8500
        /// <summary>
        /// Gets an <see cref="IMtType"/> at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the object, must implement IMtType.</typeparam>
        /// <param name="offset">The offset at which to retrieve the object</param>
        /// <returns>The object at the offset</returns>
        public unsafe T GetMtType<T>(nint offset) where T : IMtType
        {
            return *(T*)(Instance + offset);
        }

        /// <summary>
        /// Sets an <see cref="IMtType"/> at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the object, must implement IMtType.</typeparam>
        /// <param name="offset">The offset at which to set the object</param>
        /// <param name="value">The object</param>
        public unsafe void SetMtType<T>(nint offset, T value) where T : IMtType
        {
            *(T*)(Instance + offset) = value;
        }

        /// <inheritdoc cref="GetMtType{T}(nint)"/>
        public unsafe ref T GetMtTypeRef<T>(nint offset) where T : IMtType
        {
            return ref *(T*)(Instance + offset);
        }
#pragma warning restore CS8500

        /// <summary>
        /// Gets an object at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the object, must inherit from <see cref="NativeWrapper"/></typeparam>
        /// <param name="offset">The offset at which to retrieve the object</param>
        /// <returns>The object at the offset</returns>
        public T? GetObject<T>(nint offset) where T : NativeWrapper, new()
        {
            var ptr = Get<nint>(offset);

            return ptr != 0
                ? new T
                {
                    Instance = Get<nint>(offset)
                }
                : null;
        }

        /// <summary>
        /// Retrieves an inlined object at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the object, must inherit from <see cref="NativeWrapper"/></typeparam>
        /// <param name="offset">The offset at which to retrieve the object</param>
        /// <returns>The object at the offset</returns>
        public T GetInlineObject<T>(nint offset) where T : NativeWrapper, new()
        {
            return new T
            {
                Instance = Instance + offset
            };
        }

        /// <summary>
        /// Sets an object at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the object, must inherit from <see cref="NativeWrapper"/></typeparam>
        /// <param name="offset">The offset at which to set the object</param>
        /// <param name="value">The object to set</param>
        public void SetObject<T>(nint offset, T value) where T : NativeWrapper
        {
            Set(offset, value.Instance);
        }

        /// <summary>
        /// Gets a reference to a value in the object at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="offset">The offset of the value</param>
        /// <returns>A reference to the value at the offset</returns>
        public unsafe ref T GetRefInline<T>(nint offset) where T : unmanaged
        {
            return ref *(T*)(Instance + offset);
        }

        /// <summary>
        /// Gets a pointer to a value in the object at the specified offset.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="offset">The offset of the value</param>
        /// <returns>A pointer to the value at the offset</returns>
        public unsafe T* GetPtrInline<T>(nint offset) where T : unmanaged
        {
            return (T*)(Instance + offset);
        }

        public static bool operator ==(NativeWrapper? left, NativeWrapper? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Instance == right.Instance;
        }

        public static bool operator ==(NativeWrapper? left, nint right)
        {
            if (left is null)
                return false;

            return left.Instance == right;
        }

        public static bool operator ==(nint left, NativeWrapper? right)
        {
            if (right is null)
                return false;

            return left == right.Instance;
        }

        public static bool operator !=(NativeWrapper? left, nint right)
        {
            return !(left == right);
        }

        public static bool operator !=(nint left, NativeWrapper? right)
        {
            return !(left == right);
        }

        public static bool operator !=(NativeWrapper? left, NativeWrapper? right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is NativeWrapper other && this == other;
        }

        public override int GetHashCode()
        {
            return Instance.GetHashCode();
        }
    }
}
