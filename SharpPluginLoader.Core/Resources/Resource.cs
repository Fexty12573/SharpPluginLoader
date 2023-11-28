using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Resources
{
    /// <summary>
    /// Represents an instance of the cResource class.
    /// </summary>
    public class Resource : MtObject
    {
        public Resource(nint instance) : base(instance) => AddRef();
        public Resource() { }
        ~Resource() => Release();

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


        private static readonly NativeAction<nint, nint> AddRefFunc = new(AddressRepository.Get("ResourceManager:AddRef"));
        private static readonly NativeAction<nint, nint> ReleaseFunc = new(AddressRepository.Get("ResourceManager:Release"));
    }
}
