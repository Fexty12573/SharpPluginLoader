using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Resources
{
    /// <summary>
    /// Represents an instance of the cResource class.
    /// </summary>
    public class Resource : MtObject
    {
        public Resource(nint instance, bool weakRef = false) : base(instance)
        {
            _isWeakRef = weakRef;
            if (!_isWeakRef)
                AddRef();
        }

        public Resource()
        {
            _isWeakRef = true;
        }

        ~Resource()
        {
            if (!_isWeakRef)
                Release();
        }

        /// <summary>
        /// Increments the reference count of this resource.
        /// </summary>
        /// <remarks>
        /// This class automatically increments and decrements the reference counter when it is created/destroyed.
        /// You should not have to call this method explicitly.
        /// </remarks>
        public unsafe void AddRef() => AddRefFunc.Invoke(ResourceManager.SingletonInstance, Instance);

        /// <summary>
        /// Decrements the reference count of this resource. If the reference count reaches 0, the resource is unloaded.
        /// </summary>
        /// <remarks>
        /// This class automatically increments and decrements the reference counter when it is created/destroyed.
        /// You should not have to call this method explicitly.
        /// </remarks>
        public unsafe void Release() => ReleaseFunc.Invoke(ResourceManager.SingletonInstance, Instance);

        /// <summary>
        /// Gets the file path of this resource without the extension.
        /// </summary>
        public unsafe string FilePath => new(GetPtrInline<sbyte>(0xC));

        /// <summary>
        /// Gets the file extension of this resource.
        /// </summary>
        public unsafe string FileExtension => new((sbyte*)new NativeFunction<nint>(GetVirtualFunction(6)).Invoke());

        /// <summary>
        /// Gets the reference count of this resource. If the reference count reaches 0, the resource is unloaded.
        /// </summary>
        public uint RefCount => Get<uint>(0x5C);

        /// <summary>
        /// Deserializes this resource from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to deserialize this resource from.</param>
        /// <returns>True if the resource was deserialized successfully, false otherwise.</returns>
        public unsafe bool LoadFrom(MtStream stream)
        {
            return new NativeFunction<nint, nint, bool>(GetVirtualFunction(10)).Invoke(Instance, stream.Instance);
        }

        /// <summary>
        /// Serializes this resource to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to serialize this resource to.</param>
        /// <returns>True if the resource was serialized successfully, false otherwise.</returns>
        public unsafe bool SaveTo(MtStream stream)
        {
            return new NativeFunction<nint, nint, bool>(GetVirtualFunction(11)).Invoke(Instance, stream.Instance);
        }

        public T GetWeakRef<T>() where T : Resource, new()
        {
            return new T { Instance = Instance };
        }

        private readonly bool _isWeakRef;

        private static readonly NativeAction<nint, nint> AddRefFunc = new(AddressRepository.Get("ResourceManager:AddRef"));
        private static readonly NativeAction<nint, nint> ReleaseFunc = new(AddressRepository.Get("ResourceManager:Release"));
    }
}
