using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
{
    public class NativeWrapper
    {
        public nint Instance { get; internal init; }

        public NativeWrapper(nint instance)
        {
            Instance = instance;
        }

        public NativeWrapper()
        {
            Instance = 0;
        }

        public unsafe T Get<T>(nint offset) where T : unmanaged
        {
            return *(T*)(Instance + offset);
        }

        public unsafe void Set<T>(nint offset, T value) where T : unmanaged
        {
            *(T*)(Instance + offset) = value;
        }

        public unsafe ref T GetRef<T>(nint offset) where T : unmanaged
        {
            return ref *(T*)(Instance + offset);
        }

#pragma warning disable CS8500
        public unsafe T GetMtType<T>(nint offset) where T : IMtType
        {
            return *(T*)(Instance + offset);
        }

        public unsafe void SetMtType<T>(nint offset, T value) where T : IMtType
        {
            *(T*)(Instance + offset) = value;
        }

        public unsafe ref T GetMtTypeRef<T>(nint offset) where T : IMtType
        {
            return ref *(T*)(Instance + offset);
        }
#pragma warning restore CS8500

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

        public T GetInlineObject<T>(nint offset) where T : NativeWrapper, new()
        {
            return new T
            {
                Instance = Instance + offset
            };
        }

        public void SetObject<T>(nint offset, T value) where T : NativeWrapper
        {
            Set(offset, value.Instance);
        }

        public unsafe ref T GetRefInline<T>(nint offset) where T : unmanaged
        {
            return ref *(T*)(Instance + offset);
        }

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
