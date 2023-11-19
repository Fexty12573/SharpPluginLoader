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
        public unsafe void AddRef() => AddRefFunc.Invoke(Instance);

        /// <summary>
        /// Decrements the reference count of this resource. If the reference count reaches 0, the resource is unloaded.
        /// </summary>
        /// <remarks>
        /// This class automatically increments and decrements the reference counter when it is created/destroyed.
        /// You should not have to call this method explicitly.
        /// </remarks>
        public unsafe void Release() => ReleaseFunc.Invoke(Instance);

        /// <summary>
        /// Gets the file path of this resource without the extension.
        /// </summary>
        public unsafe string FilePath => new(GetPtrInline<sbyte>(0xC));

        /// <summary>
        /// Gets the file extension of this resource.
        /// </summary>
        public unsafe string FileExtension => new((sbyte*)new NativeFunction<nint>(GetVirtualFunction(6)).Invoke());

        private static readonly NativeAction<nint> AddRefFunc = new(0x142215a60);
        private static readonly NativeAction<nint> ReleaseFunc = new(0x1422160f0);
    }
}
