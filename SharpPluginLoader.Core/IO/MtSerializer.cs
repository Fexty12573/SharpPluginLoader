using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO;

[StructLayout(LayoutKind.Explicit, Size = 0x378)]
public readonly unsafe ref struct MtSerializer()
{
    public T? DeserializeBinary<T>(MtStream stream, ushort version, T dst,
        SerializerMode mode = SerializerMode.State) where T : MtObject
    {
        fixed (MtSerializer* serializer = &this)
        {
            var result = _deserializeBinary.Invoke(
                (nint)serializer,
                stream.Instance,
                version,
                dst.Instance,
                mode
            );

            return result == 0 ? null : dst;
        }
    }

    public T? DeserializeXml<T>(MtStream stream, string rootName, T dst,
        SerializerMode mode = SerializerMode.State, SerializerEncoding encoding = SerializerEncoding.Auto)
        where T : MtObject
    {
        fixed (MtSerializer* serializer = &this)
        {
            var result = _deserializeXml.Invoke(
                (nint)serializer,
                stream.Instance,
                rootName,
                dst.Instance,
                mode,
                encoding
            );

            return result == 0 ? null : dst;
        }
    }

    public bool SerializeBinary(MtStream dst, MtObject src, ushort version, SerializerMode mode = SerializerMode.State)
    {
        fixed (MtSerializer* serializer = &this)
        {
            return _serializeBinary.Invoke(
                (nint)serializer,
                dst.Instance,
                version,
                src.Instance,
                mode,
                0
            );
        }
    }

    public bool SerializeXml(MtStream dst, MtObject src, string rootName, SerializerEncoding encoding = SerializerEncoding.Auto)
    {
        fixed (MtSerializer* serializer = &this)
        {
            return _serializeXml.Invoke(
                (nint)serializer,
                dst.Instance,
                rootName,
                src.Instance,
                encoding
            );
        }
    }


    [FieldOffset(0x100)] private readonly int _maxClassId = 0x10000;
    [FieldOffset(0x370)] private readonly bool _370 = false;

    private static readonly NativeFunction<nint, nint, ushort, nint, SerializerMode, nint> _deserializeBinary =
        new(AddressRepository.Get("MtSerializer:DeserializeBinary"));
    private static readonly NativeFunction<nint, nint, string, nint, SerializerMode, SerializerEncoding, nint> _deserializeXml =
        new(AddressRepository.Get("MtSerializer:DeserializeXml"));
    private static readonly NativeFunction<nint, nint, ushort, nint, SerializerMode, nint, bool> _serializeBinary =
        new(AddressRepository.Get("MtSerializer:SerializeBinary"));
    private static readonly NativeFunction<nint, nint, string, nint, SerializerEncoding, bool> _serializeXml =
        new(AddressRepository.Get("MtSerializer:SerializeXml"));
}

public enum SerializerMode
{
    State = 0,
    Config = 1,
    User = 2
}

public enum SerializerEncoding
{
    Auto = 0,
    AsIs = 1
}
