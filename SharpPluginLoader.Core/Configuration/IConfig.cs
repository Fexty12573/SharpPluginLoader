using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Configuration
{
    /// <summary>
    /// Represents a configuration file.
    /// </summary>
    /// <remarks>
    /// All classes that implement this interface must have a public parameterless constructor (or a default constructor).
    /// </remarks>
    public interface IConfig
    {
        /// <summary>
        /// The name of the configuration file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The version of the configuration file.
        /// </summary>
        public string Version { get; }
    }
}
