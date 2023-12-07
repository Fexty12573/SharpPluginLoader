namespace SharpPluginLoader.Core
{
    public class MtObject : NativeWrapper
    {
        public MtObject(nint instance) : base(instance) { }
        public MtObject() { }

        public T As<T>() where T : MtObject, new() => new() { Instance = Instance };

        public bool Is(string typeName) => GetDti()?.InheritsFrom(typeName) ?? false;

        private unsafe nint* VTable => GetPtr<nint>(0x0);

        /// <summary>
        /// Gets a virtual function from the vtable of this object.
        /// </summary>
        /// <param name="index">The index of the virtual function in the vtable</param>
        /// <returns>The requested virtual function</returns>
        public unsafe nint GetVirtualFunction(int index)
        {
            return VTable[index];
        }

        /// <summary>
        /// Gets the properties of this object.
        /// </summary>
        /// <returns>The property list containing all properties</returns>
        public MtPropertyList GetProperties()
        {
            var propList = MtPropertyList.Dti.CreateInstance<MtPropertyList>();

            PopulatePropertyList(propList);
            propList.Deleter = obj => obj.Destroy(true);
            return propList;
        }

        /// <summary>
        /// Gets the DTI of this object.
        /// </summary>
        /// <returns>The DTI, or null if there is no DTI</returns>
        public unsafe MtDti? GetDti()
        {
            var dti = ((delegate* unmanaged<nint>)GetVirtualFunction(4))();
            return dti != 0 ? new MtDti(dti) : null;
        }

        /// <summary>
        /// Populates the specified property list with the properties of this object.
        /// </summary>
        /// <param name="list">The property list to be populated</param>
        internal unsafe void PopulatePropertyList(MtPropertyList list)
        {
            ((delegate* unmanaged<nint, nint, void>)GetVirtualFunction(3))(Instance, list.Instance);
        }

        /// <summary>
        /// Calls the destructor of this object.
        /// </summary>
        /// <param name="free">Whether the destructor should deallocate the object or not</param>
        public unsafe void Destroy(bool free)
        {
            ((delegate* unmanaged<nint, bool, void>)GetVirtualFunction(0))(Instance, free);
        }
    }
}
