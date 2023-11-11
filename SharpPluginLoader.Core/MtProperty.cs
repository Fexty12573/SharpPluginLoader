namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Represents a property of a <see cref="MtObject"/>.
    /// </summary>
    public class MtProperty : NativeWrapper
    {
        public MtProperty(nint instance) : base(instance) { }
        public MtProperty() { }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public unsafe string Name => new(GetPtr<sbyte>(0x0));

        /// <summary>
        /// An optional comment for the property.
        /// </summary>
        public unsafe string Comment => Get<nint>(0x8) != 0 ? new string(GetPtr<sbyte>(0x8)) : string.Empty;

        /// <summary>
        /// The type of the property.
        /// </summary>
        public PropType Type => (PropType)(Get<uint>(0x10) & 0xFFF);

        /// <summary>
        /// The attributes of the property.
        /// </summary>
        public uint Attr => Get<uint>(0x10) >> 12;

        /// <summary>
        /// The owning object of the property.
        /// </summary>
        public MtObject? Owner => GetObject<MtObject>(0x18);

        /// <summary>
        /// The getter method of the property. (If it has one)
        /// </summary>
        public nint Get => Get<nint>(0x20);

        /// <summary>
        /// The GetCount method of the property. (If it is a dynamic array)
        /// </summary>
        public nint GetCount => Get<nint>(0x28);

        /// <summary>
        /// The setter method of the property. (If it has one)
        /// </summary>
        public nint Set => Get<nint>(0x30);

        /// <summary>
        /// The realloc method of the property. (If it is a dynamic array)
        /// </summary>
        public nint Realloc => Get<nint>(0x38);

        public uint Index => Get<uint>(0x40);

        /// <summary>
        /// The previous property in the object's property list.
        /// </summary>
        public MtProperty? Previous => GetObject<MtProperty>(0x48);

        /// <summary>
        /// The next property in the object's property list.
        /// </summary>
        public MtProperty? Next => GetObject<MtProperty>(0x50);
    }
}
