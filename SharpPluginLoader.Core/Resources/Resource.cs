namespace SharpPluginLoader.Core.Resources
{
    public class Resource : MtObject
    {
        public Resource(nint instance) : base(instance) => AddRef();
        public Resource() { }
        ~Resource() => Release();

        public unsafe void AddRef() => AddRefFunc.Invoke(Instance);

        public unsafe void Release() => ReleaseFunc.Invoke(Instance);

        public unsafe string FilePath => new(GetPtrInline<sbyte>(0xC));

        public unsafe string FileExtension => new((sbyte*)new NativeFunction<nint>(GetVirtualFunction(6)).Invoke());

        private static readonly NativeAction<nint> AddRefFunc = new(0x142215a60);
        private static readonly NativeAction<nint> ReleaseFunc = new(0x1422160f0);
    }
}
