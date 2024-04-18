using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Iced.Intel;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Dti;

internal static class DtiExtensions
{
    public static unsafe MtDti RegisterDti(
        string name, 
        long size,
        NativeDtiDtorDelegate dtor,
        NativeDtiNewDelegate @new,
        NativeDtiCtorDelegate ctor,
        NativeDtiCtorArrayDelegate ctorArray,
        MtDti? parent = null, 
        uint attr = 0, 
        int allocatorIndex = 0)
    {
        var dtiObj = MemoryUtil.Alloc(DtiSize);
        var namePtr = Utf8StringMarshaller.ConvertToUnmanaged(name);

        DtiInitFunc.Invoke(
            dtiObj,
            (nint)namePtr,
            parent?.Instance ?? 0,
            size,
            0,
            attr,
            allocatorIndex
        );

        var vtable = NativeArray<nint>.Create(4);
        vtable[0] = Marshal.GetFunctionPointerForDelegate(dtor);
        vtable[1] = Marshal.GetFunctionPointerForDelegate(@new);
        vtable[2] = Marshal.GetFunctionPointerForDelegate(ctor);
        vtable[3] = Marshal.GetFunctionPointerForDelegate(ctorArray);

        MemoryUtil.GetRef<nint>(dtiObj) = vtable.Address;
        Utf8StringMarshaller.Free(namePtr);

        CustomDtiList.Add(new CustomDtiRecord()
        {
            Instance = dtiObj,
            Dtor = dtor,
            New = @new,
            Ctor = ctor,
            CtorArray = ctorArray,
            Vtable = vtable
        });

        return new MtDti(dtiObj);
    }

    private const int DtiSize = 0x38;

    private static readonly List<CustomDtiRecord> CustomDtiList = [];
    private static readonly NativeAction<nint, nint, nint, long, uint, uint, int> DtiInitFunc =
        new(AddressRepository.Get("MtDti:Init"));

    private readonly struct CustomDtiRecord
    {
        public required nint Instance { get; init; }
        public required NativeDtiDtorDelegate Dtor { get; init; }
        public required NativeDtiNewDelegate New { get; init; }
        public required NativeDtiCtorDelegate Ctor { get; init; }
        public required NativeDtiCtorArrayDelegate CtorArray { get; init; }

        public required NativeArray<nint> Vtable { get; init; }
    }
}

