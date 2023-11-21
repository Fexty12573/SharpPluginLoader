using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Experimental
{
    /// <summary>
    /// A custom marshaller for a pair of managed and native types
    /// </summary>
    /// <typeparam name="TManaged">The managed type</typeparam>
    /// <typeparam name="TNative">The native type</typeparam>
    public interface ICustomMarshaller<TManaged, TNative>
    {
        /// <summary>
        /// Marshals a native type to a managed type
        /// </summary>
        /// <param name="native">The native input value</param>
        /// <returns>The marshalled managed value</returns>
        public TManaged NativeToManaged(TNative native);

        /// <summary>
        /// Marshals a managed type to a native type
        /// </summary>
        /// <param name="managed">The managed input value</param>
        /// <returns>The marshalled native value</returns>
        public TNative ManagedToNative(TManaged managed);
    }
}
