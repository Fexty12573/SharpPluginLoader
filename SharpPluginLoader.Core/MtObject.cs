using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Memory.Windows;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// This class represents the base of most game objects, and serves as a wrapper around the native object.
    /// </summary>
    public class MtObject : NativeWrapper
    {
        public MtObject(nint instance) : base(instance) { }
        public MtObject() { }

        /// <summary>
        /// Checks if the given address points to an instance (or subclass) of MtObject. Does not make any allocations.
        /// </summary>
        /// <remarks>
        /// Note: This function cannot be relied upon 100% of the time. It does not
        /// validate the passed in pointer besides a null-check. If you pass in an
        /// invalid (but non-zero) address, this will likely result in an access violation.
        /// </remarks>
        /// <param name="addr">The address to check</param>
        /// <param name="checkDtiGetter">Whether to also check the DTI getter function. More accurate but slighly slower.</param>
        /// <returns>True if addr points to an instance of MtObject.</returns>
        public static bool IsInstance(nint addr, bool checkDtiGetter = true)
        {
            if (addr == 0)
                return false;

            var (baseAddr, size) = WinApi.GetExecutableExtents();
            if (size == 0)
                return false;

            // MtObject vtables are always at least 0x28 bytes in size (>=5 entries)
            var potentialVtable = MemoryUtil.Read<nint>(addr);
            if (potentialVtable < baseAddr || potentialVtable + 0x28 > baseAddr + size)
                return false;

            if (!checkDtiGetter)
                return true;

            var potentialGetDti = MemoryUtil.Read<nint>(potentialVtable + 0x20);
            if (potentialGetDti < baseAddr || potentialGetDti + 9 > baseAddr + size)
                return false;

            // GetDti functions always have the form
            // lea rax, [<dti>]
            // ret
            // So we can pattern match on that.
            return MemoryUtil.AsSpan<byte>(potentialGetDti, 8) switch
            {
                [0x48, 0x8D, 0x05, _, _, _, _, 0xC3] => true,
                _ => false,
            };
        }

        /// <summary>
        /// Casts this object to the specified MtObject subclass.
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <returns>The casted object</returns>
        /// <remarks>
        /// This method isn't actually a cast, but rather a construction of a new object of the specified type.
        /// This is because the actual type of the object is unknown at runtime. That means that the new object will
        /// have the same instance as this object.
        ///
        /// It is also important to note that this method does not perform any type checking. It is recommended to first
        /// check the type of the object using the <see cref="Is"/> method before casting, unless you are certain that the
        /// object is of the correct type.
        /// </remarks>
        public T As<T>() where T : MtObject, new() => new() { Instance = Instance };

        /// <summary>
        /// Checks if this object is of the specified type.
        /// </summary>
        /// <param name="typeName">The name of the type to check for</param>
        /// <returns></returns>
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

        public override string ToString()
        {
            var dti = GetDti();
            return dti is null ? $"0x{Instance:X}" : $"{dti.Name} @ 0x{Instance:X}";
        }
    }
}
