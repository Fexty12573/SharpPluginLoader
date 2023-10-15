using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public class MtObject : NativeWrapper
    {
        public MtObject(nint instance) : base(instance) { }
        public MtObject() { }

        private unsafe nint* VTable => (nint*)Get<nint>(0x0);

        public unsafe nint GetVirtualFunction(int index)
        {
            return VTable[index];
        }

        public MtPropertyList? GetProperties()
        {
            var propList = MtDti.Find("MtPropertyList")?.CreateInstance<MtPropertyList>();
            if (propList == null)
                return null;

            unsafe
            {
                ((delegate* unmanaged[Fastcall]<nint, nint, void>)GetVirtualFunction(3))(Instance, propList.Instance);
                propList.Deleter = obj => ((delegate* unmanaged[Fastcall]<nint, bool, void>)obj.GetVirtualFunction(0))(propList.Instance, false);
                return propList;
            }
        }

        public MtDti GetDti()
        {
            unsafe
            {
                return new MtDti(((delegate* unmanaged[Fastcall]<nint>)GetVirtualFunction(4))());
            }
        }

        public static bool operator ==(MtObject? left, MtObject? right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Instance == right.Instance;
        }

        public static bool operator !=(MtObject? left, MtObject? right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is MtObject other && this == other;
        }

        public override int GetHashCode()
        {
            return Instance.GetHashCode();
        }
    }
}
