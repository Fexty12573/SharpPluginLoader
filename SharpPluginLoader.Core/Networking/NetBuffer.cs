using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Networking
{
    /// <summary>
    /// Represents a network buffer used for reading and writing data.
    /// </summary>
    public unsafe class NetBuffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetBuffer"/> class with the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use.</param>
        public NetBuffer(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer.ToArray();
            Size = buffer.Length;
            _position = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetBuffer"/> class with the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer to use.</param>
        public NetBuffer(byte[] buffer)
        {
            _buffer = buffer;
            Size = buffer.Length;
            _position = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetBuffer"/> class with the specified size.
        /// </summary>
        /// <param name="size">The size of the buffer.</param>
        public NetBuffer(int size)
        {
            _buffer = new byte[size];
            Size = size;
            _position = 0;
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        public ReadOnlySpan<byte> Buffer => _buffer.AsSpan();

        /// <summary>
        /// Gets the size of the buffer.
        /// </summary>
        public int Size { get; }

        #region Write

        /// <summary>
        /// Writes a byte to the buffer and advances the position by 1.
        /// </summary>
        /// <param name="value">The byte value to write.</param>
        public void WriteByte(byte value)
        {
            if (_position + 1 > Size)
                throw new IndexOutOfRangeException();

            _buffer[_position++] = value;
        }

        /// <summary>
        /// Writes an unsigned 16-bit integer to the buffer and advances the position by 2.
        /// </summary>
        /// <param name="value">The unsigned 16-bit integer value to write.</param>
        public void WriteUInt16(ushort value)
        {
            if (_position + 2 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<ushort>() = BinaryPrimitives.ReverseEndianness(value);
            _position += 2;
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer to the buffer and advances the position by 4.
        /// </summary>
        /// <param name="value">The unsigned 32-bit integer value to write.</param>
        public void WriteUInt32(uint value)
        {
            if (_position + 4 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<uint>() = BinaryPrimitives.ReverseEndianness(value);
            _position += 4;
        }

        /// <summary>
        /// Writes an unsigned 64-bit integer to the buffer and advances the position by 8.
        /// </summary>
        /// <param name="value">The unsigned 64-bit integer value to write.</param>
        public void WriteUInt64(ulong value)
        {
            if (_position + 8 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<ulong>() = BinaryPrimitives.ReverseEndianness(value);
            _position += 8;
        }

        /// <summary>
        /// Writes a signed byte to the buffer and advances the position by 1.
        /// </summary>
        /// <param name="value">The signed byte value to write.</param>
        public void WriteSByte(sbyte value)
        {
            if (_position + 1 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<sbyte>() = value;
            _position += 1;
        }

        /// <summary>
        /// Writes a signed 16-bit integer to the buffer and advances the position by 2.
        /// </summary>
        /// <param name="value">The signed 16-bit integer value to write.</param>
        public void WriteInt16(short value)
        {
            if (_position + 2 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<short>() = BinaryPrimitives.ReverseEndianness(value);
            _position += 2;
        }

        /// <summary>
        /// Writes a signed 32-bit integer to the buffer and advances the position by 4.
        /// </summary>
        /// <param name="value">The signed 32-bit integer value to write.</param>
        public void WriteInt32(int value)
        {
            if (_position + 4 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<int>() = BinaryPrimitives.ReverseEndianness(value);
            _position += 4;
        }

        /// <summary>
        /// Writes a signed 64-bit integer to the buffer and advances the position by 8.
        /// </summary>
        /// <param name="value">The signed 64-bit integer value to write.</param>
        public void WriteInt64(long value)
        {
            if (_position + 8 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<long>() = BinaryPrimitives.ReverseEndianness(value);
            _position += 8;
        }

        /// <summary>
        /// Writes a single-precision floating-point value to the buffer and advances the position by 4.
        /// </summary>
        /// <param name="value">The single-precision floating-point value to write.</param>
        public void WriteFloat(float value)
        {
            WriteUInt32(*(uint*)&value);
        }

        /// <summary>
        /// Writes a double-precision floating-point value to the buffer and advances the position by 8.
        /// </summary>
        /// <param name="value">The double-precision floating-point value to write.</param>
        public void WriteDouble(double value)
        {
            WriteUInt64(*(ulong*)&value);
        }

        /// <summary>
        /// Writes a boolean value to the buffer and advances the position by 1.
        /// </summary>
        /// <param name="value">The boolean value to write.</param>
        public void WriteBoolean(bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <summary>
        /// Writes a byte array to the buffer and advances the position by the length of the array.
        /// </summary>
        /// <param name="bytes">The byte array to write.</param>
        public void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            if (_position + bytes.Length > Size)
                throw new IndexOutOfRangeException();

            bytes.CopyTo(_buffer.AsSpan(_position));
            _position += bytes.Length;
        }

        /// <summary>
        /// Writes a string encoded in UTF-8 to the buffer and advances the position by the length of the string.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        public void WriteString(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var bytes = Encoding.UTF8.GetBytes(value);
            if (_position + bytes.Length > Size)
                throw new ArgumentOutOfRangeException(nameof(value));

            WriteBytes(bytes);
        }

        /// <summary>
        /// Writes a string encoded in the specified encoding to the buffer and advances the position by the length of the string.
        /// </summary>
        /// <param name="value">The string value to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        public void WriteString(string value, Encoding encoding)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentNullException.ThrowIfNull(encoding);

            var bytes = encoding.GetBytes(value);
            if (_position + bytes.Length > Size)
                throw new ArgumentOutOfRangeException(nameof(value));

            WriteBytes(bytes);
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads a byte from the buffer and advances the position by 1.
        /// </summary>
        /// <returns>The byte value read from the buffer.</returns>
        public byte ReadByte()
        {
            if (_position + 1 > Size)
                throw new IndexOutOfRangeException();

            return _buffer[_position++];
        }

        /// <summary>
        /// Reads an unsigned 16-bit integer from the buffer and advances the position by 2.
        /// </summary>
        /// <returns>The unsigned 16-bit integer value read from the buffer.</returns>
        public ushort ReadUInt16()
        {
            if (_position + 2 > Size)
                throw new IndexOutOfRangeException();

            var value = BinaryPrimitives.ReverseEndianness(*Ptr<ushort>());
            _position += 2;
            return value;
        }

        /// <summary>
        /// Reads an unsigned 32-bit integer from the buffer and advances the position by 4.
        /// </summary>
        /// <returns>The unsigned 32-bit integer value read from the buffer.</returns>
        public uint ReadUInt32()
        {
            if (_position + 4 > Size)
                throw new IndexOutOfRangeException();

            var value = BinaryPrimitives.ReverseEndianness(*Ptr<uint>());
            _position += 4;
            return value;
        }

        /// <summary>
        /// Reads an unsigned 64-bit integer from the buffer and advances the position by 8.
        /// </summary>
        /// <returns>The unsigned 64-bit integer value read from the buffer.</returns>
        public ulong ReadUInt64()
        {
            if (_position + 8 > Size)
                throw new IndexOutOfRangeException();

            var value = BinaryPrimitives.ReverseEndianness(*Ptr<ulong>());
            _position += 8;
            return value;
        }

        /// <summary>
        /// Reads a signed byte from the buffer and advances the position by 1.
        /// </summary>
        /// <returns>The signed byte value read from the buffer.</returns>
        public sbyte ReadSByte()
        {
            if (_position + 1 > Size)
                throw new IndexOutOfRangeException();

            var value = *Ptr<sbyte>();
            _position += 1;
            return value;
        }

        /// <summary>
        /// Reads a signed 16-bit integer from the buffer and advances the position by 2.
        /// </summary>
        /// <returns>The signed 16-bit integer value read from the buffer.</returns>
        public short ReadInt16()
        {
            if (_position + 2 > Size)
                throw new IndexOutOfRangeException();

            var value = BinaryPrimitives.ReverseEndianness(*Ptr<short>());
            _position += 2;
            return value;
        }

        /// <summary>
        /// Reads a signed 32-bit integer from the buffer and advances the position by 4.
        /// </summary>
        /// <returns>The signed 32-bit integer value read from the buffer.</returns>
        public int ReadInt32()
        {
            if (_position + 4 > Size)
                throw new IndexOutOfRangeException();

            var value = BinaryPrimitives.ReverseEndianness(*Ptr<int>());
            _position += 4;
            return value;
        }

        /// <summary>
        /// Reads a signed 64-bit integer from the buffer and advances the position by 8.
        /// </summary>
        /// <returns>The signed 64-bit integer value read from the buffer.</returns>
        public long ReadInt64()
        {
            if (_position + 8 > Size)
                throw new IndexOutOfRangeException();

            var value = BinaryPrimitives.ReverseEndianness(*Ptr<long>());
            _position += 8;
            return value;
        }

        /// <summary>
        /// Reads a single-precision floating-point value from the buffer and advances the position by 4.
        /// </summary>
        /// <returns>The single-precision floating-point value read from the buffer.</returns>
        public float ReadFloat()
        {
            var value = ReadUInt32();
            return *(float*)&value;
        }

        /// <summary>
        /// Reads a double-precision floating-point value from the buffer and advances the position by 8.
        /// </summary>
        /// <returns>The double-precision floating-point value read from the buffer.</returns>
        public double ReadDouble()
        {
            var value = ReadUInt64();
            return *(double*)&value;
        }

        /// <summary>
        /// Reads a boolean value from the buffer and advances the position by 1.
        /// </summary>
        /// <returns>The boolean value read from the buffer.</returns>
        public bool ReadBoolean()
        {
            return ReadByte() != 0;
        }

        /// <summary>
        /// Reads a specified number of bytes from the buffer and advances the position by the number of bytes read.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>A byte array containing the bytes read from the buffer.</returns>
        public byte[] ReadBytes(int count)
        {
            if (_position + count > Size)
                throw new IndexOutOfRangeException();

            var bytes = new byte[count];
            Array.Copy(_buffer, _position, bytes, 0, count);
            _position += count;
            return bytes;
        }

        /// <summary>
        /// Reads a string from the buffer encoded in UTF-8 and advances the position by the length of the string.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The string value read from the buffer.</returns>
        public string ReadString(int count)
        {
            if (_position + count > Size)
                throw new IndexOutOfRangeException();

            var value = Encoding.UTF8.GetString(_buffer, _position, count);
            _position += count;
            return value;
        }

        /// <summary>
        /// Reads a string from the buffer encoded in the specified encoding and advances the position by the length of the string.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The string value read from the buffer.</returns>
        public string ReadString(int count, Encoding encoding)
        {
            if (_position + count > Size)
                throw new IndexOutOfRangeException();

            var value = encoding.GetString(_buffer, _position, count);
            _position += count;
            return value;
        }

        #endregion

        #region Internal

        internal void WriteInt32NoBSwap(int value)
        {
            if (_position + 4 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<int>() = value;
            _position += 4;
        }

        internal void WriteInt64NoBSwap(long value)
        {
            if (_position + 8 > Size)
                throw new IndexOutOfRangeException();

            *Ptr<long>() = value;
            _position += 8;
        }

        internal NetBuffer(NativeWrapper nativeInstance) 
            : this(new ReadOnlySpan<byte>(nativeInstance.GetPtr<byte>(0x48), nativeInstance.Get<int>(0x50)))
        {
            _position = nativeInstance.Get<int>(0x54);
        }

        private T* Ptr<T>() where T : unmanaged
        {
            return (T*)(BufferPtr + _position);
        }

        private byte* BufferPtr => (byte*)Unsafe.AsPointer(ref _buffer[0]);
        private readonly byte[] _buffer;
        private int _position;

        internal static MtObject CreateNative(int size)
        {
            var instance = MemoryUtil.Alloc(0x60);
            var buffer = MemoryUtil.Alloc(size);
            Ctor.Invoke(instance);
            Create.Invoke(instance, buffer, size);
            return new MtObject(instance);
        }

        internal static MtObject CreateNative(ReadOnlySpan<byte> buffer)
        {
            var instance = MemoryUtil.Alloc(0x60);
            var nativeBuffer = MemoryUtil.Alloc(buffer.Length);
            Ctor.Invoke(instance);
            Create.Invoke(instance, nativeBuffer, buffer.Length);
            Marshal.Copy(buffer.ToArray(), 0, nativeBuffer, buffer.Length);
            return new MtObject(instance);
        }

        internal static void FreeNative(MtObject instance)
        {
            MemoryUtil.Free(instance.GetPtr<byte>(0x48));
            MemoryUtil.Free(instance.Instance);
        }

        private static readonly NativeFunction<nint, nint> Ctor = new(AddressRepository.Get("NetBuffer:Ctor"));
        private static readonly NativeAction<nint, nint, int> Create = new(AddressRepository.Get("NetBuffer:Create"));
        #endregion
    }
}
