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

        public unsafe MtPropertyList? GetProperties()
        {
            var dti = MtDti.Find("MtPropertyList");
            if (dti == null)
                return null;

            var propList = dti.CreateInstance<MtPropertyList>();
            ((delegate* unmanaged<nint, nint, void>)GetVirtualFunction(3))(Instance, propList.Instance);
            propList.Deleter = obj => ((delegate* unmanaged<nint, bool, void>)obj.GetVirtualFunction(0))(propList.Instance, false);
            return propList;
        }

        public unsafe MtDti? GetDti()
        {
            var dti = ((delegate* unmanaged<nint>)GetVirtualFunction(4))();
            return dti != 0 ? new MtDti(dti) : null;
        }
    }
}
